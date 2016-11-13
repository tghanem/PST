using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.encoders
{
    class BIEncoder : IEncoder<BI>
    {
        public BinaryData Encode(BI value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value.Value));
        }
    }
}
