namespace pst.encodables
{
    class BREF
    {
        public BID BlockId { get; }

        public BI ByteIndex { get; }

        private BREF(BID blockId, BI byteIndex)
        {
            BlockId = blockId;
            ByteIndex = byteIndex;
        }

        public static BREF OfValue(BID blockId, BI byteIndex)
            => new BREF(blockId, byteIndex);
    }
}
