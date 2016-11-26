using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HNPAGEHDRDecoder : IDecoder<HNPAGEHDR>
    {
        private readonly IDecoder<int> int32Decoder;

        public HNPAGEHDRDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public HNPAGEHDR Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new HNPAGEHDR(
                        parser.TakeAndSkip(2, int32Decoder));
            }
        }
    }
}
