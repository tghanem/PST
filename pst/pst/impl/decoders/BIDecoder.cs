using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.decoders
{
    class BIDecoder : IDecoder<BI>
    {
        public BI Decode(BinaryData encodedData)
        {
            return BI.OfValue(BitConverter.ToInt64(encodedData.Value, 0));
        }
    }
}
