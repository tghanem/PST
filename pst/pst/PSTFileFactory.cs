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
                        dataBlockReader,
                        new NIDDecoder()),
                    CreateTCReader(
                        dataBlockReader,
                        new TagDecoder()),
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    new NIDDecoder(),
                    CreateSubnodesEnumerator(
                        dataBlockReader),
                    new PropertyNameToIdMap(
                        new NAMEIDDecoder(),
                        CreatePCBasedPropertyReader(
                            dataBlockReader),
                        new DictionaryBasedMapper<NID, LNBTEntry>(nodeBTree)),
                    CreatePCBasedPropertyReader(
                        dataBlockReader),
                    new DictionaryBasedMapper<NID, LNBTEntry>(nodeBTree));
        }

        private static TCReader<TRowId> CreateTCReader<TRowId>(IDataBlockReader dataBlockReader, IDecoder<TRowId> rowIdDecoder) where TRowId : IComparable<TRowId>
        {
            return
                new TCReader<TRowId>(rowIdDecoder,
                    CreateRowIndexReader(
                        dataBlockReader,
                        rowIdDecoder),
                    CreateRowMatrixReader(
                        rowIdDecoder,
                        dataBlockReader),
                    CreatePropertyValueProcessor(
                        dataBlockReader));
        }

        private static IRowIndexReader<TRowId> CreateRowIndexReader<TRowId>(IDataBlockReader dataBlockReader, IDecoder<TRowId> rowIdDecoder) where TRowId : IComparable<TRowId>
        {
            return
                new RowIndexReader<TRowId>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(
                        dataBlockReader),
                    CreateBTreeOnHeapReader(
                        rowIdDecoder,
                        dataBlockReader),
                    new DataRecordToTCROWIDConverter());
        }

        private static IPCBasedPropertyReader CreatePCBasedPropertyReader(IDataBlockReader dataBlockReader)
        {
            return
                new PCBasedPropertyReader(
                    CreateBTreeOnHeapReader(
                        new PropertyIdDecoder(),
                        dataBlockReader),
                    CreatePropertyValueProcessor(dataBlockReader));
        }

        private static IPropertyValueProcessor CreatePropertyValueProcessor(IDataBlockReader dataBlockReader)
        {
            return
                new PropertyValueProcessor(
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new ExternalDataBlockDecoder(
                        new BlockTrailerDecoder(
                            new BIDDecoder()),
                        new PermutativeDecoder(false)),
                    CreateHeapOnNodeReader(
                        dataBlockReader),
                    CreateSubnodesEnumerator(
                        dataBlockReader),
                    CreateDataTreeLeafNodesEnumerator(
                        dataBlockReader),
                    new PropertyTypeMetadataProvider(),
                    dataBlockReader);
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

        private static IRowMatrixReader<TRowId> CreateRowMatrixReader<TRowId>(IDecoder<TRowId> rowIdDecoder, IDataBlockReader dataBlockReader) where TRowId : IComparable<TRowId>
        {
            return
                new RowMatrixReader<TRowId>(
                    CreateHeapOnNodeReader(
                        dataBlockReader),
                    new RowValuesExtractor(),
                    CreateSubnodesEnumerator(
                        dataBlockReader),
                    new RowIndexReader<TRowId>(
                        new TCINFODecoder(
                            new HIDDecoder(),
                            new TCOLDESCDecoder()),
                        CreateHeapOnNodeReader(
                            dataBlockReader),
                        CreateBTreeOnHeapReader(
                            rowIdDecoder,
                            dataBlockReader),
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

        private static SubNodesEnumerator CreateSubnodesEnumerator(IDataBlockReader dataBlockReader)
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

        private static IBTreeOnHeapReader<TKey> CreateBTreeOnHeapReader<TKey>(IDecoder<TKey> keyDecoder, IDataBlockReader dataBlockReader) where TKey : IComparable<TKey>
        {
            return
                new BTreeOnHeapReader<TKey>(
                    new HIDDecoder(),
                    keyDecoder,
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(
                        dataBlockReader));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(IDataBlockReader dataBlockReader)
        {
            return
                new HeapOnNodeReader(
                    new HNHDRDecoder(
                        new HIDDecoder()),
                    new HNPAGEHDRDecoder(),
                    new HNPAGEMAPDecoder(),
                    new PermutativeDecoder(false),
                    new HNBITMAPHDRDecoder(),
                    new HeapOnNodeItemsLoader(),
                    CreateDataTreeLeafNodesEnumerator(
                        dataBlockReader),
                    dataBlockReader);
        }

        private static IDataTreeLeafBIDsEnumerator CreateDataTreeLeafNodesEnumerator(IDataBlockReader dataBlockReader)
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

        private static IBTreeLeafKeysEnumerator<LNBTEntry, BREF> CreateNodeBTreeLeafKeysEnumerator(IDataReader dataReader)
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

        private static IBTreeLeafKeysEnumerator<LBBTEntry, BREF> CreateBlockBTreeLeafKeysEnumerator(IDataReader dataReader)
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
