using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.encoders
{
    class BIEncoder : IEncoder<IB>
    {
        public BinaryData Encode(IB value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value.Value));
        }
    }
}
