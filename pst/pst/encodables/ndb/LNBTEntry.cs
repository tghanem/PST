using pst.utilities;

namespace pst.encodables.ndb
{
    class LNBTEntry
    {
        public NID NodeId { get; }

        public BID DataBlockId { get; }

        public BID SubnodeBlockId { get; }

        public NID ParentNodeId { get; }

        public BinaryData Padding { get; }

        public LNBTEntry(NID nodeId, BID dataBlockId, BID subnodeBlockId, NID parentNodeId, BinaryData padding)
        {
            NodeId = nodeId;
            DataBlockId = dataBlockId;
            SubnodeBlockId = subnodeBlockId;
            ParentNodeId = parentNodeId;
            Padding = padding;
        }
    }
}
