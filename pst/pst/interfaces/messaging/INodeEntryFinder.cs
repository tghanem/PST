using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    class NodeEntry
    {
        public NodeEntry(BID nodeDataBlockId, BID subnodeDataBlockId)
        {
            NodeDataBlockId = nodeDataBlockId;
            SubnodeDataBlockId = subnodeDataBlockId;
        }

        public BID NodeDataBlockId { get; }

        public BID SubnodeDataBlockId { get; }
    }

    interface INodeEntryFinder
    {
        Maybe<NodeEntry> GetEntry(NID[] nodePath);
    }
}