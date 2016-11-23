using pst.encodables;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders
{
    class BTPageDecoder : IDecoder<BTPage>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<PageTrailer> pageTrailerDecoder;

        public BTPageDecoder(IDecoder<int> int32Decoder, IDecoder<PageTrailer> pageTrailerDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.pageTrailerDecoder = pageTrailerDecoder;
        }

        public BTPage Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new BTPage(
                        parser.TakeAndSkip(488),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(4),
                        parser.TakeAndSkip(16, pageTrailerDecoder));
            }
        }
    }
}
