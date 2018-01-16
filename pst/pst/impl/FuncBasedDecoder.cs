using pst.interfaces;
using pst.utilities;
using System;

namespace pst.impl
{
    class FuncBasedDecoder<T> : IDecoder<T>
    {
        private readonly Func<BinaryData, T> decode;

        public FuncBasedDecoder(Func<BinaryData, T> decode)
        {
            this.decode = decode;
        }

        public T Decode(BinaryData encodedData) => decode(encodedData);
    }
}
