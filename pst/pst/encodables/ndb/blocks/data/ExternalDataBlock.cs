using pst.utilities;

namespace pst.encodables.ndb.blocks.data
{
    class ExternalDataBlock
    {
        public BinaryData Data { get; }

        public BinaryData Padding { get; }

        ///16
        public BlockTrailer Trailer { get; }

        public ExternalDataBlock(BinaryData data, BinaryData padding, BlockTrailer trailer)
        {
            Data = data;
            Padding = padding;
            Trailer = trailer;
        }
    }
}
