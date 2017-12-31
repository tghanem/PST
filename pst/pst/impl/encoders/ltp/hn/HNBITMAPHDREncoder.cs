using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ltp.hn
{
    class HNBITMAPHDREncoder : IEncoder<HNBITMAPHDR>
    {
        public BinaryData Encode(HNBITMAPHDR value)
        {
            return
                BinaryDataGenerator
                .New()
                .Append((short)value.PageMapOffset)
                .Append(value.FillLevel)
                .GetData();
        }
    }
}
