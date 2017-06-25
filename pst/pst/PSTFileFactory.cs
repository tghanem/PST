using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.btree;
using pst.impl.converters;
using pst.impl.decoders;
using pst.impl.decoders.ltp.bth;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.ltp.tc;
using pst.impl.decoders.messaging;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.blocks;
using pst.impl.decoders.ndb.blocks.data;
using pst.impl.decoders.ndb.blocks.subnode;
using pst.impl.decoders.ndb.btree;
using pst.impl.io;
using pst.impl.ltp;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.impl.ltp.pc;
using pst.impl.ltp.tc;
using pst.impl.messaging;
using pst.impl.ndb;
using pst.impl.ndb.bbt;
using pst.impl.ndb.datatree;
using pst.impl.ndb.nbt;
using pst.impl.ndb.subnodebtree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var dataReader = new DataReader(stream);

            var nodeBTree = new Dictionary<NID, LNBTEntry>();

            var blockBTree = new Dictionary<BID, LBBTEntry>();

            var header = CreateHeaderDecoder().Decode(dataReader.Read(0, 546));
            
            foreach (var entry in CreateNodeBTreeLeafKeysEnumerator(dataReader)
                                  .Enumerate(header.Root.NBTRootPage))
            {
                nodeBTree.Add(entry.NodeId, entry);
            }

            foreach (var entry in CreateBlockBTreeLeafKeysEnumerator(dataReader)
                                  .Enumerate(header.Root.BBTRootPage))
            {
                blockBTree.Add(entry.BlockReference.BlockId, entry);
            }

            var dataBlockReader =
                new BlockIdBasedDataBlockReader(
                    dataReader,
                    new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree));

            return
                new PSTFile(
                    CreateTCReader(
                        new NIDDecoder(),
                        dataBlockReader,
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    CreateTCReader(
                        new TagDecoder(),
                        dataBlockReader,
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    new NIDDecoder(),
                    CreateSubnodesEnumerator(
                        dataBlockReader),
                    new PropertyNameToIdMap(
                        new NAMEIDDecoder(),
                        CreatePCBasedPropertyReader(
                            dataBlockReader,
                            CreateBlockDataDeObfuscator(
                                header.CryptMethod)),
                        new DictionaryBasedMapper<NID, LNBTEntry>(nodeBTree)),
                    CreatePCBasedPropertyReader(
                        dataBlockReader,
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    new DictionaryBasedMapper<NID, LNBTEntry>(nodeBTree));
        }

        private static IDecoder<Header> CreateHeaderDecoder()
        {
            return
                new HeaderDecoder(
                    new RootDecoder(
                        new BREFDecoder(
                            new BIDDecoder(),
                            new IBDecoder())),
                    new BIDDecoder(),
                    new NIDDecoder());
        }

        private static IBlockDataDeObfuscator CreateBlockDataDeObfuscator(
            int cryptMethod)
        {
            if (cryptMethod == Globals.NDB_CRYPT_NONE)
            {
                return new NoEncoding();
            }
            else if (cryptMethod == Globals.NDB_CRYPT_CYCLIC)
            {
                return new CyclicEncoding();
            }
            else if (cryptMethod == Globals.NDB_CRYPT_PERMUTE)
            {
                return new PermutativeEncoding();
            }

            throw new Exception($"Unexpected bCryptMethod {cryptMethod}");
        }

        private static ITableContextReader<TRowId> CreateTCReader<TRowId>(
            IDecoder<TRowId> rowIdDecoder,
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator) where TRowId : IComparable<TRowId>
        {
            return
                new TableContextReader<TRowId>(rowIdDecoder,
                    CreateRowIndexReader(rowIdDecoder,
                        dataBlockReader, blockDataDeObfuscator),
                    CreateRowMatrixReader(
                        rowIdDecoder,
                        dataBlockReader,
                        blockDataDeObfuscator),
                    CreatePropertyValueProcessor(
                        dataBlockReader,
                        blockDataDeObfuscator));
        }

        private static IRowIndexReader<TRowId> CreateRowIndexReader<TRowId>(
            IDecoder<TRowId> rowIdDecoder,
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator) where TRowId : IComparable<TRowId>
        {
            return
                new RowIndexReader<TRowId>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(
                        dataBlockReader,
                        blockDataDeObfuscator),
                    CreateBTreeOnHeapReader(
                        rowIdDecoder,
                        dataBlockReader,
                        blockDataDeObfuscator),
                    new DataRecordToTCROWIDConverter());
        }

        private static IPCBasedPropertyReader CreatePCBasedPropertyReader(
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator)
        {
            return
                new PCBasedPropertyReader(
                    CreateBTreeOnHeapReader(
                        new PropertyIdDecoder(),
                        dataBlockReader,
                        blockDataDeObfuscator),
                    CreatePropertyValueProcessor(
                        dataBlockReader,
                        blockDataDeObfuscator));
        }

        private static IPropertyValueProcessor CreatePropertyValueProcessor(
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator)
        {
            return
                new PropertyValueProcessor(
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new ExternalDataBlockDecoder(
                        new BlockTrailerDecoder(
                            new BIDDecoder()),
                        new PermutativeEncoding()),
                    CreateHeapOnNodeReader(
                        dataBlockReader,
                        blockDataDeObfuscator),
                    CreateSubnodesEnumerator(
                        dataBlockReader),
                    CreateDataTreeLeafNodesEnumerator(
                        dataBlockReader),
                    new PropertyTypeMetadataProvider(),
                    dataBlockReader);
        }

        private static IRowMatrixReader<TRowId> CreateRowMatrixReader<TRowId>(
            IDecoder<TRowId> rowIdDecoder,
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator) where TRowId : IComparable<TRowId>
        {
            return
                new RowMatrixReader<TRowId>(
                    CreateHeapOnNodeReader(
                        dataBlockReader,
                        blockDataDeObfuscator),
                    new RowValuesExtractor(),
                    CreateSubnodesEnumerator(
                        dataBlockReader),
                    new RowIndexReader<TRowId>(
                        new TCINFODecoder(
                            new HIDDecoder(),
                            new TCOLDESCDecoder()),
                        CreateHeapOnNodeReader(
                            dataBlockReader,
                            blockDataDeObfuscator),
                        CreateBTreeOnHeapReader(
                            rowIdDecoder,
                            dataBlockReader,
                            blockDataDeObfuscator),
                        new DataRecordToTCROWIDConverter()),
                    CreateDataTreeLeafNodesEnumerator(
                        dataBlockReader),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    dataBlockReader);
        }

        private static ISubNodesEnumerator CreateSubnodesEnumerator(
            IDataBlockReader dataBlockReader)
        {
            return
                new SubNodesEnumerator(
                    new SubnodeBTreeBlockLevelDecider(
                        dataBlockReader),
                    new SubnodeBlockLoader(
                        new SubnodeBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder())),
                        dataBlockReader),
                    new SIEntriesFromSubnodeBlockExtractor(
                        new SIEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new SLEntriesFromSubnodeBlockExtractor(
                        new SLEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())));
        }

        private static IBTreeOnHeapReader<TKey> CreateBTreeOnHeapReader<TKey>(
            IDecoder<TKey> keyDecoder,
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator) where TKey : IComparable<TKey>
        {
            return
                new BTreeOnHeapReader<TKey>(
                    new HIDDecoder(),
                    keyDecoder,
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(
                        dataBlockReader,
                        blockDataDeObfuscator));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            IDataBlockReader dataBlockReader,
            IBlockDataDeObfuscator blockDataDeObfuscator)
        {
            return
                new HeapOnNodeReader(
                    new HNHDRDecoder(
                        new HIDDecoder()),
                    new HNPAGEHDRDecoder(),
                    new HNPAGEMAPDecoder(),
                    new HNBITMAPHDRDecoder(),
                    blockDataDeObfuscator,
                    new HeapOnNodeItemsLoader(),
                    CreateDataTreeLeafNodesEnumerator(
                        dataBlockReader),
                    dataBlockReader);
        }

        private static IDataTreeLeafBIDsEnumerator CreateDataTreeLeafNodesEnumerator(
            IDataBlockReader dataBlockReader)
        {
            return
                new DataTreeLeafBIDsEnumerator(
                    new DataTreeBlockLevelDecider(
                        dataBlockReader),
                    new InternalDataBlockLoader(
                        new InternalDataBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder())),
                        dataBlockReader),
                    new BIDsFromInternalDataBlockExtractor(
                        new BIDDecoder()));
        }

        private static IBTreeLeafKeysEnumerator<LNBTEntry, BREF> CreateNodeBTreeLeafKeysEnumerator(
            IDataReader dataReader)
        {
            return
                new BTreeLeafKeysEnumerator<BTPage, BREF, INBTEntry, LNBTEntry>(
                    new BREFFromINBTEntryExtractor(),
                    new INBTEntriesFromBTPageExtractor(
                        new INBTEntryDecoder(
                            new NIDDecoder(),
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new LNBTEntriesFromBTPageExtractor(
                        new LNBTEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new PageLevelFromBTPageExtractor(),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }

        private static IBTreeLeafKeysEnumerator<LBBTEntry, BREF> CreateBlockBTreeLeafKeysEnumerator(
            IDataReader dataReader)
        {
            return
                new BTreeLeafKeysEnumerator<BTPage, BREF, IBBTEntry, LBBTEntry>(
                    new BREFFromIBBTEntryExtractor(),
                    new IBBTEntriesFromBTPageExtractor(
                        new IBBTEntryDecoder(
                            new BIDDecoder(),
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new LBBTEntriesFromBTPageExtractor(
                        new LBBTEntryDecoder(
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new PageLevelFromBTPageExtractor(),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }
    }
}
