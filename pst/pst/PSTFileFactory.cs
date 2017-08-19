using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl;
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
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var dataReader = new DataReader(stream);

            var header = CreateHeaderDecoder().Decode(dataReader.Read(0, 546));

            return
                new PSTFile(
                    CreateTCReader(
                        new NIDDecoder(),
                        CreateDataBlockReader(dataReader),
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    CreateTCReader(
                        new TagDecoder(),
                        CreateDataBlockReader(dataReader),
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    new NIDDecoder(),
                    CreateSubnodesEnumerator(
                        CreateDataBlockReader(dataReader)),
                    CreatePropertyIdToNameMap(
                        dataReader,
                        header),
                    CreatePCBasedPropertyReader(
                        CreateDataBlockReader(dataReader),
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    CreateNIDToLNBTEntryMapper(dataReader));
        }

        private static BlockIdBasedDataBlockReader CreateDataBlockReader(
            IDataReader dataReader)
        {
            return
                new BlockIdBasedDataBlockReader(
                    dataReader,
                    CreateHeaderDecoder(),
                    CreateBlockBTreeEntryFinder(
                        dataReader));
        }

        private static NIDToLNBTEntryMapper CreateNIDToLNBTEntryMapper(
            IDataReader dataReader)
        {
            return
                new NIDToLNBTEntryMapper(
                    dataReader,
                    CreateHeaderDecoder(),
                    CreateNodeBTreeEntryFinder(
                        dataReader));
        }

        private static PropertyNameToIdMap CreatePropertyIdToNameMap(
            IDataReader dataReader,
            Header header)
        {
            return
                new PropertyNameToIdMap(
                    new NAMEIDDecoder(),
                    CreatePCBasedPropertyReader(
                        new BlockIdBasedDataBlockReader(
                            dataReader,
                            CreateHeaderDecoder(),
                            CreateBlockBTreeEntryFinder(
                                dataReader)),
                        CreateBlockDataDeObfuscator(
                            header.CryptMethod)),
                    CreateNIDToLNBTEntryMapper(
                        dataReader));
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

            if (cryptMethod == Globals.NDB_CRYPT_CYCLIC)
            {
                return new CyclicEncoding();
            }

            if (cryptMethod == Globals.NDB_CRYPT_PERMUTE)
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
                        blockDataDeObfuscator),
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

        private static IBTreeEntryFinder<BID, LBBTEntry, BREF> CreateBlockBTreeEntryFinder(
            IDataReader dataReader)
        {
            return
                new BTreeEntryFinder<BID, LBBTEntry, IBBTEntry, BREF, BTPage>(
                    new FuncBasedExtractor<LBBTEntry, BID>(
                        entry => entry.BlockReference.BlockId),
                    new FuncBasedExtractor<IBBTEntry, BID>(
                        entry => entry.Key),
                    new FuncBasedExtractor<IBBTEntry, BREF>(
                        entry => entry.ChildPageBlockReference),
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
                    new FuncBasedExtractor<BTPage, int>(
                        page => page.PageLevel),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }

        private static IBTreeEntryFinder<NID, LNBTEntry, BREF> CreateNodeBTreeEntryFinder(
            IDataReader dataReader)
        {
            return
                new BTreeEntryFinder<NID, LNBTEntry, INBTEntry, BREF, BTPage>(
                    new FuncBasedExtractor<LNBTEntry, NID>(
                        entry => entry.NodeId),
                    new FuncBasedExtractor<INBTEntry, NID>(
                        entry => entry.Key),
                    new FuncBasedExtractor<INBTEntry, BREF>(
                        entry => entry.ChildPageBlockReference),
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
                    new FuncBasedExtractor<BTPage, int>(
                        page => page.PageLevel),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }
    }
}
