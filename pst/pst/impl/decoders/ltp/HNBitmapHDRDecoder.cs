using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp;

namespace pst.impl.decoders.ltp
{
    class HNBITMAPHDRDecoder : IDecoder<HNBITMAPHDR>
    {
        private readonly IDecoder<int> int32Decoder;

        public HNBITMAPHDRDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public HNBITMAPHDR Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new HNBITMAPHDR(
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(64));
            }
        }
    }
}
