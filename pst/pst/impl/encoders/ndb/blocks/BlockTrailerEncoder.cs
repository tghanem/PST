using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb;

namespace pst.impl.encoders.ndb.blocks
{
    class BlockTrailerEncoder : IEncoder<BlockTrailer>
    {
        private readonly IEncoder<BID> bidEncoder;

        private readonly IEncoder<int> int32Encoder;

        public BlockTrailerEncoder(IEncoder<BID> bidEncoder, IEncoder<int> int32Encoder)
        {
            this.bidEncoder = bidEncoder;
            this.int32Encoder = int32Encoder;
        }

        public BinaryData Encode(BlockTrailer value)
        {
            var generator = BinaryDataGenerator.New();

            return
                generator
                .Append(value.AmountOfData, int32Encoder, 2)
                .Append(value.BlockSignature, int32Encoder, 2)
                .Append(value.DataCrc, int32Encoder)
                .Append(value.BlockId, bidEncoder)
                .GetData();
        }
    }
}
