using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.bth
{
    class BTHHEADERDecoder : IDecoder<BTHHEADER>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<HID> hidDecoder;

        public BTHHEADERDecoder(IDecoder<int> int32Decoder, IDecoder<HID> hidDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.hidDecoder = hidDecoder;
        }

        public BTHHEADER Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BTHHEADER(
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(4, hidDecoder));
        }
    }
}
