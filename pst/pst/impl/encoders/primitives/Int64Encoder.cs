using pst.interfaces;
using System;
using pst.utilities;

namespace pst.impl.encoders.primitives
{
    class Int64Encoder : IEncoder<long>
    {
        public BinaryData Encode(long value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value));
        }
    }
}
