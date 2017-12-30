using pst.encodables.ndb;
using pst.encodables.ndb.blocks;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ndb.blocks
{
    class BlockTrailerEncoder : IEncoder<BlockTrailer>
    {
        private readonly IEncoder<BID> bidEncoder;

        public BlockTrailerEncoder(IEncoder<BID> bidEncoder)
        {
            this.bidEncoder = bidEncoder;
        }

        public BinaryData Encode(BlockTrailer value)
        {
            return
                BinaryDataGenerator.New()
                .Append((short)value.AmountOfData)
                .Append((short)value.BlockSignature)
                .Append(value.DataCrc)
                .Append(value.BlockId, bidEncoder)
                .GetData();
        }
    }
}
