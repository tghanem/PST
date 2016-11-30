namespace pst.encodables.ndb.blocks
{
    class BlockTrailer
    {
        ///2
        public int AmountOfData { get; }

        ///2
        public int BlockSignature { get; }

        ///4
        public int DataCrc { get; }

        ///8
        public BID BlockId { get; }

        public BlockTrailer(int amountOfData, int blockSignature, int dataCrc, BID blockId)
        {
            AmountOfData = amountOfData;
            BlockSignature = blockSignature;
            DataCrc = dataCrc;
            BlockId = blockId;
        }
    }
}
