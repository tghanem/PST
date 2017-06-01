using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HIDDecoder : IDecoder<HID>
    {
        public HID Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var value = parser.TakeAndSkip(4).ToInt32();

            return
                new HID(
                    value & 0x0000001F,
                    (value >> 5) & 0x000003FF,
                    (value >> 16) & 0x0000FFFF);
        }
    }
}
