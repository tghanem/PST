using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.hn
{
    class HNPAGEMAPDecoder : IDecoder<HNPAGEMAP>
    {
        public HNPAGEMAP Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var allocationCount = parser.TakeAndSkip(2).ToInt32();
            var freeCount = parser.TakeAndSkip(2).ToInt32();
            var allocationTable = parser.TakeAndSkip((allocationCount + 1) * 2);

            return
                new HNPAGEMAP(
                    allocationCount,
                    freeCount,
                    allocationTable);
        }
    }
}
