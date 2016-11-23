using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using System;

namespace pst.impl.encoders.ndb
{
    class BIDEncoder : IEncoder<BID>
    {
        public BinaryData Encode(BID value)
        {
            return BinaryData.OfValue(BitConverter.GetBytes(value.Value));
        }
    }
}
