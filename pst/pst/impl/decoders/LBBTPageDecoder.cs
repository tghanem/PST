using pst.encodables;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders
{
    class LBBTPageDecoder : IDecoder<LBBTPage>
    {
        private readonly IDecoder<LBBTEntry> entryDecoder;

        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<PageTrailer> pageTrailerDecoder;

        public LBBTPageDecoder(IDecoder<LBBTEntry> entryDecoder, IDecoder<int> int32Decoder, IDecoder<PageTrailer> pageTrailerDecoder)
        {
            this.entryDecoder = entryDecoder;
            this.int32Decoder = int32Decoder;
            this.pageTrailerDecoder = pageTrailerDecoder;
        }

        public LBBTPage Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new LBBTPage(
                        parser.TakeAndSkip(
                            numberOfItems:
                                parser.TakeAtWithoutChangingStreamPosition(488, 1, int32Decoder),
                            itemSize:
                                parser.TakeAtWithoutChangingStreamPosition(490, 1, int32Decoder),
                            typeDecoder:
                                entryDecoder),
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
