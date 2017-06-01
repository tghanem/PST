using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HNPAGEHDRDecoder : IDecoder<HNPAGEHDR>
    {
        public HNPAGEHDR Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new HNPAGEHDR(
                    parser.TakeAndSkip(2).ToInt32());
        }
    }
}
