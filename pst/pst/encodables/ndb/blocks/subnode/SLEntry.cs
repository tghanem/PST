using System;

namespace pst.encodables.ndb.blocks.subnode
{
    class SLEntry : IComparable<SLEntry>
    {
        ///8
        public NID LocalSubnodeId { get; }

        ///8
        public BID DataBlockId { get; }

        ///8
        public BID SubnodeBlockId { get; }

        public SLEntry(NID localSubnodeId, BID dataBlockid, BID subnodeDataBlockId)
        {
            LocalSubnodeId = localSubnodeId;
            DataBlockId = dataBlockid;
            SubnodeBlockId = subnodeDataBlockId;
        }

        public int CompareTo(SLEntry other)
        {
            return LocalSubnodeId.CompareTo(other.LocalSubnodeId);
        }
    }
}
