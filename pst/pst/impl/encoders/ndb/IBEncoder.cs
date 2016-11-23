using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using System;

namespace pst.impl.encoders.ndb
{
    class IBEncoder : IEncoder<IB>
    {
        public BinaryData Encode(IB value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value.Value));
        }
    }
}
