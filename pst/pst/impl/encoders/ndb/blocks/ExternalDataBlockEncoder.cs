using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ndb.blocks
{
    class ExternalDataBlockEncoder : IEncoder<ExternalDataBlock>
    {
        private readonly IEncoder<BlockTrailer> blockTrailerEncoder;

        public ExternalDataBlockEncoder(IEncoder<BlockTrailer> blockTrailerEncoder)
        {
            this.blockTrailerEncoder = blockTrailerEncoder;
        }

        public BinaryData Encode(ExternalDataBlock value)
        {
            return
                BinaryDataGenerator.New()
                .Append(value.Data)
                .Append(value.Padding)
                .Append(value.Trailer, blockTrailerEncoder)
                .GetData();
        }
    }
}
