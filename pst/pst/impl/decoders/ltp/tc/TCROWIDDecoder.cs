using pst.encodables.ltp.tc;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ltp.tc
{
    class TCROWIDDecoder : IDecoder<TCROWID>
    {
        private readonly IDecoder<int> int32Decoder;

        public TCROWIDDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public TCROWID Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new TCROWID(
                    parser.TakeAndSkip(4),
                    parser.TakeAndSkip(4, int32Decoder));
        }
    }
}
