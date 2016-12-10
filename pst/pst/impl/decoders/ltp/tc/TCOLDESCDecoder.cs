using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.tc;

namespace pst.impl.decoders.ltp.tc
{
    class TCOLDESCDecoder : IDecoder<TCOLDESC>
    {
        private readonly IDecoder<int> int32Decoder;

        public TCOLDESCDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public TCOLDESC Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new TCOLDESC(
                    parser.TakeAndSkip(4, int32Decoder),
                    parser.TakeAndSkip(2, int32Decoder),
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(1, int32Decoder));
        }
    }
}
