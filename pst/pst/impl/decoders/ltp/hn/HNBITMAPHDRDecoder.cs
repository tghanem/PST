using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HNBITMAPHDRDecoder : IDecoder<HNBITMAPHDR>
    {
        public HNBITMAPHDR Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new HNBITMAPHDR(
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(64));
        }
    }
}
