using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.encoders
{
    class BIDEncoder : IEncoder<BID>
    {
        public BinaryData Encode(BID value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value.Value));
        }
    }
}
