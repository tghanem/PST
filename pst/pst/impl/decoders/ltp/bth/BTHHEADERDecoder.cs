using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ltp.bth
{
    class BTHHEADERDecoder : IDecoder<BTHHEADER>
    {
        public BTHHEADER Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BTHHEADER(
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    HID.OfValue(parser.TakeAndSkip(4)));
        }
    }
}
