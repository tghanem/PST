using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ltp.hn
{
    class HNHDRDecoder : IDecoder<HNHDR>
    {
        public HNHDR Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new HNHDR(
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    HID.OfValue(parser.TakeAndSkip(4)),
                    parser.TakeAndSkip(4));
        }
    }
}
