using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using System.Linq;

namespace pst.impl.ndb
{
    class NodeEntryFinder : INodeEntryFinder
    {
        private readonly IHeaderReader headerReader;
        private readonly IBTreeEntryFinder<NID, LNBTEntry, BREF> nodeBTreeEntryFinder;
        private readonly ISubNodesEnumerator subnodesEnumerator;

        public NodeEntryFinder(
            IHeaderReader headerReader,
            IBTreeEntryFinder<NID, LNBTEntry, BREF> nodeBTreeEntryFinder,
            ISubNodesEnumerator subnodesEnumerator)
        {
            this.headerReader = headerReader;
            this.nodeBTreeEntryFinder = nodeBTreeEntryFinder;
            this.subnodesEnumerator = subnodesEnumerator;
        }

        public Maybe<NodeEntry> GetEntry(NodePath nodePath)
        {
            if (nodePath.Length == 0)
            {
                return Maybe<NodeEntry>.NoValue();
            }

            var header = headerReader.GetHeader();

            var lnbtEntry = nodeBTreeEntryFinder.Find(nodePath[0], header.Root.NBTRootPage);

            if (nodePath.Length > 1)
            {
                return GetEntry(nodePath, 1, lnbtEntry.Value.SubnodeBlockId);
            }

            var subnodes =
                lnbtEntry.Value.SubnodeBlockId.Equals(BID.Zero)
                ? new SLEntry[0]
                : subnodesEnumerator.Enumerate(lnbtEntry.Value.SubnodeBlockId);

            return new NodeEntry(lnbtEntry.Value.DataBlockId, lnbtEntry.Value.SubnodeBlockId, subnodes);
        }

        private Maybe<NodeEntry> GetEntry(
            NodePath nodePath,
            int currentDepth,
            BID parentNodeSubnodeDataBlockId)
        {
            var parentSubnodes =
                subnodesEnumerator.Enumerate(parentNodeSubnodeDataBlockId);

            var subnodeEntry =
                parentSubnodes.First(s => s.LocalSubnodeId.Equals(nodePath[currentDepth]));

            if (currentDepth < nodePath.Length - 1)
            {
                return GetEntry(nodePath, currentDepth + 1, subnodeEntry.SubnodeBlockId);
            }

            var subnodes =
                subnodeEntry.SubnodeBlockId.Equals(BID.Zero)
                ? new SLEntry[0]
                : subnodesEnumerator.Enumerate(subnodeEntry.SubnodeBlockId);

            return Maybe<NodeEntry>.OfValue(new NodeEntry(subnodeEntry.DataBlockId, subnodeEntry.SubnodeBlockId, subnodes));
        }
    }
}
