using pst.interfaces;
using pst.utilities;
using System;

namespace pst.impl.decoders.primitives
{
    class Int64Decoder : IDecoder<long>
    {
        public long Decode(BinaryData encodedData)
        {
            return BitConverter.ToInt64(encodedData.Value, 1);
        }
    }
}
