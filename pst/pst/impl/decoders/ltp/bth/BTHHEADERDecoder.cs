using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.bth
{
    class BTHHEADERDecoder : IDecoder<BTHHEADER>
    {
        private readonly IDecoder<HID> hidDecoder;

        public BTHHEADERDecoder(IDecoder<HID> hidDecoder)
        {
            this.hidDecoder = hidDecoder;
        }

        public BTHHEADER Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BTHHEADER(
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(4, hidDecoder));
        }
    }
}
