using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using System;

namespace pst.impl.decoders.ndb
{
    class IBDecoder : IDecoder<IB>
    {
        public IB Decode(BinaryData encodedData)
        {
            return IB.OfValue(BitConverter.ToInt64(encodedData.Value, 0));
        }
    }
}
