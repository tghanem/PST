using pst.interfaces;
using pst.utilities;
using System;

namespace pst.impl.decoders.primitives
{
    class Int32Decoder : IDecoder<int>
    {
        public int Decode(BinaryData encodedData)
        {
            return BitConverter.ToInt32(encodedData.Value, 0);
        }
    }
}
