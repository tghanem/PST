using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.impl.btree;
using pst.impl.converters;
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
using pst.interfaces.ndb;
using pst.utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst
{
    class PSTServiceFactory
    {
        public static IMapper<NID, SLEntry> GetMapperForSubnodes(
            IReadOnlyDictionary<BID, LBBTEntry> blockBTree,
            IDataBlockReader<BREF> streamReader,
            BID subnodeBlockId)
        {
            if (subnodeBlockId.Value == 0)
            {
                return
                    new DictionaryBasedMapper<NID, SLEntry>(
                        new Dictionary<NID, SLEntry>());
            }
            else
            {
                var bbtEntryForSubnode = blockBTree[subnodeBlockId];

                return
                    new DictionaryBasedMapper<NID, SLEntry>(
                        CreateSubnodeBTreeLeafKeysEnumerator()
                        .Enumerate(
                            new LBBTEntryBlockReaderAdapter(streamReader),
                            new SIEntryToLBBTEntryMapper(blockBTree),
                            bbtEntryForSubnode)
                        .ToDictionary(
                            k => k.LocalSubnodeId,
                            k => k));
            }
        }

        public static IDecoder<NID> CreateNIDDecoder()
        {
            return new NIDDecoder();
        }

        public static IDecoder<Header> CreateHeaderDecoder()
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

        public static IRowMatrixReader<TRowId> CreateRowMatrixReader<TRowId>(IDecoder<TRowId> rowIdDecoder) where TRowId : IComparable<TRowId>
        {
            return
                new RowMatrixReader<TRowId>(
                    CreateDataTreeLeafNodesEnumerator(),
                    new RowValuesExtractor(),
                    CreateHeapOnNodeReader(),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    new RowIndexReader<TRowId>(
                        new DataRecordToTCROWIDConverter(
                            new NIDDecoder()),
                        CreateBTreeOnHeapReader(rowIdDecoder),
                        CreateHeapOnNodeReader(),
                        new TCINFODecoder(
                            new HIDDecoder(),
                            new TCOLDESCDecoder())),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()));
        }

        public static IPCBasedPropertyReader CreatePCBasedPropertyReader()
        {
            return
                new PCBasedPropertyReader(
                    CreateHeapOnNodeReader(),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    CreateBTreeOnHeapReader(
                        new PropertyIdDecoder()),
                    new PropertyTypeMetadataProvider());
        }

        public static IBTreeOnHeapReader<TKey> CreateBTreeOnHeapReader<TKey>(IDecoder<TKey> keyDecoder) where TKey : IComparable<TKey>
        {
            return
                new BTreeOnHeapReader<TKey>(
                    CreateHeapOnNodeReader(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    keyDecoder,
                    new HIDDecoder());
        }

        public static IHeapOnNodeReader CreateHeapOnNodeReader()
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
                    new DataTreeLeafNodesEnumerator(
                        new BTreeLeafKeyEnumeratorThatDoesntKnowHowToMapKeyToNodeReference<InternalDataBlock, LBBTEntry, BID, BID>(
                            new BIDsFromInternalDataBlockExtractor(
                                new BIDDecoder()),
                            new BIDsFromInternalDataBlockExtractor(
                                new BIDDecoder()),
                            new NodeLevelFromInternalDataBlockExtractor(),
                            new InternalDataBlockLoader(
                                new InternalDataBlockDecoder(
                                    new BlockTrailerDecoder(
                                        new BIDDecoder())))),
                        new ExternalDataBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder()),
                            new PermutativeDecoder(false))));
        }

        public static IDataTreeLeafNodesEnumerator CreateDataTreeLeafNodesEnumerator()
        {
            return
                new DataTreeLeafNodesEnumerator(
                    new BTreeLeafKeyEnumeratorThatDoesntKnowHowToMapKeyToNodeReference<InternalDataBlock, LBBTEntry, BID, BID>(
                        new BIDsFromInternalDataBlockExtractor(
                            new BIDDecoder()),
                        new BIDsFromInternalDataBlockExtractor(
                            new BIDDecoder()),
                        new NodeLevelFromInternalDataBlockExtractor(),
                        new InternalDataBlockLoader(
                            new InternalDataBlockDecoder(
                                new BlockTrailerDecoder(
                                    new BIDDecoder())))),
                    new ExternalDataBlockDecoder(
                        new BlockTrailerDecoder(
                            new BIDDecoder()),
                        new PermutativeDecoder(false)));
        }

        public static IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<SLEntry, SIEntry, LBBTEntry> CreateSubnodeBTreeLeafKeysEnumerator()
        {
            return
                new BTreeLeafKeyEnumeratorThatDoesntKnowHowToMapKeyToNodeReference<SubnodeBlock, LBBTEntry, SIEntry, SLEntry>(
                    new SIEntriesFromSubnodeBlockExtractor(
                        new SIEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new SLEntriesFromSubnodeBlockExtractor(
                        new SLEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new NodeLevelFromSubnodeBlockExtractor(),
                    new SubnodeBlockLoader(
                        new SubnodeBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder()))));
        }

        public static IBTreeLeafKeysEnumerator<LNBTEntry, BREF> CreateNodeBTreeLeafKeysEnumerator()
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
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }

        public static IBTreeLeafKeysEnumerator<LBBTEntry, BREF> CreateBlockBTreeLeafKeysEnumerator()
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
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }
    }
}
