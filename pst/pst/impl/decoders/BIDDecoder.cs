using pst.interfaces;
using pst.utilities;
using pst.encodables;
using System;

namespace pst.impl.decoders
{
    class BIDDecoder : IDecoder<BID>
    {
        public BID Decode(BinaryData encodedData)
        {
            return BID.OfValue(BitConverter.ToInt64(encodedData.Value, 0));
        }
    }
}
