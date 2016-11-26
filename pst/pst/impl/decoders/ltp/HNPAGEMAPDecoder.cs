using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp;

namespace pst.impl.decoders.ltp
{
    class HNPAGEMAPDecoder : IDecoder<HNPAGEMAP>
    {
        private readonly IDecoder<int> int32Decoder;

        public HNPAGEMAPDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public HNPAGEMAP Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                var allocationCount = parser.TakeAndSkip(2, int32Decoder);
                var freeCount = parser.TakeAndSkip(2, int32Decoder);
                var allocationTable = parser.TakeAndSkip((allocationCount + 1) * 2);

                return
                    new HNPAGEMAP(
                        allocationCount,
                        freeCount,
                        allocationTable);
            }
        }
    }
}
