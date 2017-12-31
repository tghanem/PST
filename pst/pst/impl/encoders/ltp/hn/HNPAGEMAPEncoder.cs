using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ltp.hn
{
    class HNPAGEMAPEncoder : IEncoder<HNPAGEMAP>
    {
        public BinaryData Encode(HNPAGEMAP value)
        {
            return
                BinaryDataGenerator
                .New()
                .Append((short)value.AllocationCount)
                .Append((short)value.FreeCount)
                .Append(value.AllocationTable)
                .GetData();
        }
    }
}
