using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using System.Linq;

namespace pst.impl.ndb
{
    class NodeEntryFinder : INodeEntryFinder
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;
        private readonly IBTreeEntryFinder<NID, LNBTEntry, BREF> nodeBTreeEntryFinder;
        private readonly ISubNodesEnumerator subnodesEnumerator;

        public NodeEntryFinder(
            IDataReader dataReader,
            IDecoder<Header> headerDecoder,
            IBTreeEntryFinder<NID, LNBTEntry, BREF> nodeBTreeEntryFinder,
            ISubNodesEnumerator subnodesEnumerator)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
            this.nodeBTreeEntryFinder = nodeBTreeEntryFinder;
            this.subnodesEnumerator = subnodesEnumerator;
        }

        public Maybe<NodeEntry> GetEntry(NodePath nodePath)
        {
            if (nodePath.Length == 0)
            {
                return Maybe<NodeEntry>.NoValue();
            }

            var header = headerDecoder.Decode(dataReader.Read(0, 546));

            var lnbtEntry = nodeBTreeEntryFinder.Find(nodePath[0], header.Root.NBTRootPage);

            if (nodePath.Length > 1)
            {
                return GetEntry(nodePath, 1, lnbtEntry.Value.SubnodeBlockId);
            }

            return new NodeEntry(lnbtEntry.Value.DataBlockId, lnbtEntry.Value.SubnodeBlockId);
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

            return Maybe<NodeEntry>.OfValue(new NodeEntry(subnodeEntry.DataBlockId, subnodeEntry.SubnodeBlockId));
        }
    }
}
