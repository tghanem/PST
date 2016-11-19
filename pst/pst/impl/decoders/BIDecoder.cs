using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.decoders
{
    class BIDecoder : IDecoder<IB>
    {
        public IB Decode(BinaryData encodedData)
        {
            return IB.OfValue(BitConverter.ToInt64(encodedData.Value, 0));
        }
    }
}
