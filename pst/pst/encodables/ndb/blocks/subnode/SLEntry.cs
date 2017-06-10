namespace pst.encodables.ndb.blocks.subnode
{
    class SLEntry
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
    }
}
