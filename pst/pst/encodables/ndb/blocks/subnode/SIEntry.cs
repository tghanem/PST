namespace pst.encodables.ndb.blocks.subnode
{
    class SIEntry
    {
        ///8
        public NID NodeId { get; }

        ///8
        public BID SLBlockId { get; }

        public SIEntry(NID nodeId, BID slBlockId)
        {
            NodeId = nodeId;
            SLBlockId = slBlockId;
        }
    }
}
