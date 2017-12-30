namespace pst.encodables.ndb
{
    class BREF
    {
        public BID BlockId { get; }

        public IB ByteIndex { get; }

        private BREF(BID blockId, IB byteIndex)
        {
            BlockId = blockId;
            ByteIndex = byteIndex;
        }

        public static BREF OfValue(BID blockId, IB byteIndex) => new BREF(blockId, byteIndex);
    }
}
