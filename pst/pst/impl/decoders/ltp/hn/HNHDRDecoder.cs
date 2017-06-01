using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HNHDRDecoder : IDecoder<HNHDR>
    {
        private readonly IDecoder<HID> hidDecoder;

        public HNHDRDecoder(IDecoder<HID> hidDecoder)
        {
            this.hidDecoder = hidDecoder;
        }

        public HNHDR Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new HNHDR(
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(4, hidDecoder),
                    parser.TakeAndSkip(4).ToInt32());
        }
    }
}
