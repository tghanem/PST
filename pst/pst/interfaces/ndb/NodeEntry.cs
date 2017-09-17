using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;

namespace pst.interfaces.ndb
{
    class NodeEntry
    {
        public NodeEntry(BID nodeDataBlockId, BID subnodeDataBlockId, SLEntry[] childNodes)
        {
            NodeDataBlockId = nodeDataBlockId;
            SubnodeDataBlockId = subnodeDataBlockId;
            ChildNodes = childNodes;
        }

        public BID NodeDataBlockId { get; }

        public BID SubnodeDataBlockId { get; }

        public SLEntry[] ChildNodes { get; }
    }
}