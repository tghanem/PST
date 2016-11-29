using pst.interfaces;
using pst.utilities;
using System;

namespace pst.impl.decoders.primitives
{
    class Int32Decoder : IDecoder<int>
    {
        public int Decode(BinaryData encodedData)
        {
            if (encodedData.Length < 4)
            {
                var paddedData = encodedData.Pad(4 - encodedData.Length);

                return BitConverter.ToInt32(paddedData.Value, 0);
            }
            else if (encodedData.Length == 4)
            {
                return BitConverter.ToInt32(encodedData.Value, 0);
            }
            else
            {
                throw new InvalidOperationException("Invalid data length");
            }
        }
    }
}
