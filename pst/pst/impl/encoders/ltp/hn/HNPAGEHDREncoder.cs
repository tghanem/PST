using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ltp.hn
{
    class HNPAGEHDREncoder : IEncoder<HNPAGEHDR>
    {
        public BinaryData Encode(HNPAGEHDR value)
        {
            return
                BinaryDataGenerator
                .New()
                .Append((short) value.PageMapOffset)
                .GetData();
        }
    }
}
