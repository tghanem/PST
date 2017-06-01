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
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var streamReader =
                new StreamBasedBlockReader(stream);

            var dataBlockReader =
                new LBBTEntryBlockReaderAdapter(streamReader);

            var nodeBTree = new Dictionary<NID, LNBTEntry>();

            var blockBTree = new Dictionary<BID, LBBTEntry>();

            var header =
                CreateHeaderDecoder()
                .Decode(streamReader.Read(BREF.OfValue(BID.OfValue(0), IB.Zero), 546));

            foreach (var entry in CreateNodeBTreeLeafKeysEnumerator(streamReader)
                                  .Enumerate(header.Root.NBTRootPage))
            {
                nodeBTree.Add(entry.NodeId, entry);
            }

            foreach (var entry in CreateBlockBTreeLeafKeysEnumerator(streamReader)
                                  .Enumerate(header.Root.BBTRootPage))
            {
                blockBTree.Add(entry.BlockReference.BlockId, entry);
            }

            return
                new PSTFile(
                    new MessageStore(
                        Globals.NID_MESSAGE_STORE,
                        CreatePCBasedPropertyReader(dataBlockReader),
                        new DictionaryBasedMapper<NID, LNBTEntry>(nodeBTree),
                        new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree),
                        new LBBTEntryBlockReaderAdapter(streamReader)),
                    new Folder(
                        Globals.NID_ROOT_FOLDER,
                        new RowIndexReader<NID>(
                            new DataRecordToTCROWIDConverter(
                                new NIDDecoder()),
                            CreateBTreeOnHeapReader(
                                new NIDDecoder(),
                                dataBlockReader),
                            CreateHeapOnNodeReader(dataBlockReader),
                            new TCINFODecoder(
                                new HIDDecoder(),
                                new TCOLDESCDecoder()),
                            dataBlockReader),
                        CreateRowMatrixReader(
                            new NIDDecoder(),
                            dataBlockReader),
                        CreatePCBasedPropertyReader(dataBlockReader),
                        new DictionaryBasedMapper<NID, LNBTEntry>(nodeBTree),
                        new DictionaryBasedMapper<BID, LBBTEntry>(blockBTree)));
        }

        private static IPCBasedPropertyReader CreatePCBasedPropertyReader(
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            return
                new PCBasedPropertyReader(
                    CreateHeapOnNodeReader(dataBlockReader),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    CreateBTreeOnHeapReader(
                        new PropertyIdDecoder(),
                        dataBlockReader),
                    new PropertyTypeMetadataProvider());
        }

        private static IDecoder<NID> CreateNIDDecoder()
        {
            return new NIDDecoder();
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

        private static IRowMatrixReader<TRowId> CreateRowMatrixReader<TRowId>(
            IDecoder<TRowId> rowIdDecoder,
            IDataBlockReader<LBBTEntry> dataBlockReader)
            
            where TRowId : IComparable<TRowId>
        {
            return
                new RowMatrixReader<TRowId>(
                    CreateDataTreeLeafNodesEnumerator(
                        dataBlockReader),
                    new RowValuesExtractor(),
                    CreateHeapOnNodeReader(
                        dataBlockReader),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    new RowIndexReader<TRowId>(
                        new DataRecordToTCROWIDConverter(
                            new NIDDecoder()),
                        CreateBTreeOnHeapReader(
                            rowIdDecoder,
                            dataBlockReader),
                        CreateHeapOnNodeReader(dataBlockReader),
                        new TCINFODecoder(
                            new HIDDecoder(),
                            new TCOLDESCDecoder()),
                        dataBlockReader),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    dataBlockReader);
        }

        private static IBTreeOnHeapReader<TKey> CreateBTreeOnHeapReader<TKey>(
            IDecoder<TKey> keyDecoder,
            IDataBlockReader<LBBTEntry> dataBlockReader)
            
            where TKey : IComparable<TKey>
        {
            return
                new BTreeOnHeapReader<TKey>(
                    CreateHeapOnNodeReader(dataBlockReader),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    keyDecoder,
                    new HIDDecoder(),
                    dataBlockReader);
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            IDataBlockReader<LBBTEntry> dataBlockReader)
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
                                        new BIDDecoder())),
                                dataBlockReader),
                            dataBlockReader),
                        new ExternalDataBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder()),
                            new PermutativeDecoder(false))),
                    dataBlockReader);
        }

        private static IDataTreeLeafNodesEnumerator CreateDataTreeLeafNodesEnumerator(
            IDataBlockReader<LBBTEntry> dataBlockReader)
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
                                    new BIDDecoder())),
                            dataBlockReader),
                        dataBlockReader),
                    new ExternalDataBlockDecoder(
                        new BlockTrailerDecoder(
                            new BIDDecoder()),
                        new PermutativeDecoder(false)));
        }

        private static IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<SLEntry, SIEntry, LBBTEntry> CreateSubnodeBTreeLeafKeysEnumerator(
            IDataBlockReader<LBBTEntry> dataBlockReader)
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
                                new BIDDecoder())),
                        dataBlockReader),
                    dataBlockReader);
        }

        private static IBTreeLeafKeysEnumerator<LNBTEntry, BREF> CreateNodeBTreeLeafKeysEnumerator(
            IDataBlockReader<BREF> pageBlockReader)
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
                                new BIDDecoder())),
                        pageBlockReader),
                    pageBlockReader);
        }

        private static IBTreeLeafKeysEnumerator<LBBTEntry, BREF> CreateBlockBTreeLeafKeysEnumerator(
            IDataBlockReader<BREF> pageBlockReader)
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
                                new BIDDecoder())),
                        pageBlockReader),
                    pageBlockReader);
        }
    }
}
