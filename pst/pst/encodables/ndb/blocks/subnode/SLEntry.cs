namespace pst.encodables.ndb.blocks.subnode
{
    class SLEntry
    {
        ///8
        public NID LocalSubnodeId { get; }

        ///8
        public BID DataBlockId { get; }

        ///8
        public NID SubnodeId { get; }

        public SLEntry(NID localSubnodeId, BID dataBlockid, NID subnodeId)
        {
            LocalSubnodeId = localSubnodeId;
            DataBlockId = dataBlockid;
            SubnodeId = subnodeId;
        }
    }
}
