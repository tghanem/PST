using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HNHDRDecoder : IDecoder<HNHDR>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<HID> hidDecoder;

        public HNHDRDecoder(IDecoder<int> int32Decoder, IDecoder<HID> hidDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.hidDecoder = hidDecoder;
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
                        parser.TakeAndSkip(4, hidDecoder),
                        parser.TakeAndSkip(4, int32Decoder));
            }
        }
    }
}
