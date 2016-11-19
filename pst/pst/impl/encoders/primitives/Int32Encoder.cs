using pst.interfaces;
using System;
using pst.utilities;

namespace pst.impl.encoders.primitives
{
    class Int32Encoder : IEncoder<int>
    {
        public BinaryData Encode(int value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value));
        }
    }
}
