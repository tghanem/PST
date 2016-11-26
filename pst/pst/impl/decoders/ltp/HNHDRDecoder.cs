using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp;

namespace pst.impl.decoders.ltp
{
    class HNHDRDecoder : IDecoder<HNHDR>
    {
        private readonly IDecoder<int> int32Decoder;

        public HNHDRDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public HNHDR Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new HNHDR(
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(4, int32Decoder),
                        parser.TakeAndSkip(4, int32Decoder));
            }
        }
    }
}
