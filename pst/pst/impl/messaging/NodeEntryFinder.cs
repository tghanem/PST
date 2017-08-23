using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using System.Linq;

namespace pst.impl.messaging
{
    class NodeEntryFinder : INodeEntryFinder
    {
        private readonly IMapper<NID, Maybe<LNBTEntry>> nidToLNBTEntryMapper;
        private readonly ISubNodesEnumerator subnodesEnumerator;

        public NodeEntryFinder(
            IMapper<NID, Maybe<LNBTEntry>> nidToLNBTEntryMapper,
            ISubNodesEnumerator subnodesEnumerator)
        {
            this.nidToLNBTEntryMapper = nidToLNBTEntryMapper;
            this.subnodesEnumerator = subnodesEnumerator;
        }

        public Maybe<NodeEntry> GetEntry(NID[] nodePath)
        {
            if (nodePath.Length == 0)
            {
                return Maybe<NodeEntry>.NoValue();
            }

            var lnbtEntry = nidToLNBTEntryMapper.Map(nodePath[0]);

            if (nodePath.Length > 1)
            {
                return GetEntry(nodePath, 1, lnbtEntry.Value.SubnodeBlockId);
            }

            return new NodeEntry(lnbtEntry.Value.DataBlockId, lnbtEntry.Value.SubnodeBlockId);
        }

        private Maybe<NodeEntry> GetEntry(
            NID[] nodePath,
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
