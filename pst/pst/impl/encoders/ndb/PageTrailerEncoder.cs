using pst.interfaces;
using pst.utilities;
using pst.encodables;

namespace pst.impl.encoders
{
    class PageTrailerEncoder : IEncoder<PageTrailer>
    {
        private readonly IEncoder<int> int32Encoder;

        private readonly IEncoder<BID> bidEncoder;

        public PageTrailerEncoder(IEncoder<int> int32Encoder, IEncoder<BID> bidEncoder)
        {
            this.int32Encoder = int32Encoder;
            this.bidEncoder = bidEncoder;
        }

        public BinaryData Encode(PageTrailer value)
        {
            using (var generator = BinaryDataGenerator.New())
            {
                return
                    generator
                    .Append(value.PageType, int32Encoder, 1)
                    .Append(value.PageTypeRepeat, int32Encoder, 1)
                    .Append(value.PageSignature, int32Encoder, 2)
                    .Append(value.Crc32ForPageData, int32Encoder)
                    .Append(value.PageBlockId, bidEncoder)
                    .GetData();
            }
        }
    }
}
