using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ltp.hn
{
    class HNHDREncoder : IEncoder<HNHDR>
    {
        public BinaryData Encode(HNHDR value)
        {
            return
                BinaryDataGenerator
                .New()
                .Append((short)value.PageMapOffset)
                .Append((byte)value.BlockSignature)
                .Append((byte)value.ClientSignature)
                .Append(value.UserRoot.Value)
                .Append(value.FillLevel)
                .GetData();
        }
    }
}
