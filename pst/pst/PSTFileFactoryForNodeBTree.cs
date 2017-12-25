using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl;
using pst.impl.btree;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.blocks;
using pst.impl.decoders.ndb.blocks.subnode;
using pst.impl.decoders.ndb.btree;
using pst.impl.io;
using pst.impl.ndb;
using pst.impl.ndb.cache;
using pst.impl.ndb.nbt;
using pst.impl.ndb.subnodebtree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static INodeEntryFinder CreateNodeEntryFinder(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new NodeEntryFinderThatCachesTheNodeEntry(
                    nodeEntryCache,
                    new NodeEntryFinder(
                        CreateHeaderReader(dataStream), 
                        CreateNodeBTreeEntryFinder(dataStream),
                        CreateSubnodesEnumerator(dataStream, dataBlockEntryCache)));
        }

        private static ISubNodesEnumerator CreateSubnodesEnumerator(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new SubNodesEnumerator(
                    new SubnodeBTreeBlockLevelDecider(
                        CreateDataBlockReader(dataStream, dataBlockEntryCache)),
                    new SubnodeBlockLoader(
                        new SubnodeBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder())),
                        CreateDataBlockReader(dataStream, dataBlockEntryCache)),
                    new SIEntriesFromSubnodeBlockExtractor(
                        new SIEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new SLEntriesFromSubnodeBlockExtractor(
                        new SLEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())));
        }

        private static IBTreeEntryFinder<NID, LNBTEntry, BREF> CreateNodeBTreeEntryFinder(
            Stream dataStream)
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
                        new DataReader(dataStream),
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }
    }
}
