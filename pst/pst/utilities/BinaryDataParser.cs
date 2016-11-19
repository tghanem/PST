using pst.interfaces;
using System;
using System.IO;

namespace pst.utilities
{
    class BinaryDataParser : IDisposable
    {
        private readonly MemoryStream valueStream;

        private BinaryDataParser(MemoryStream valueStream)
        {
            this.valueStream = valueStream;
        }

        public static BinaryDataParser OfValue(BinaryData data)
            => new BinaryDataParser(new MemoryStream(data.Value));

        public BinaryData TakeAndSkip(int count)
        {
            var buffer = new byte[count];

            valueStream.Read(buffer, 0, count);

            return BinaryData.OfValue(buffer);
        }

        public TType TakeAndSkip<TType>(int count, IDecoder<TType> typeDecoder)
        {
            var buffer = new byte[count];

            valueStream.Read(buffer, 0, count);

            return typeDecoder.Decode(BinaryData.OfValue(buffer));
        }

        public void Dispose()
        {
            valueStream.Dispose();
        }
    }
}
