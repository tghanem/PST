using pst.interfaces;
using System;
using System.IO;

namespace pst.utilities
{
    class BinaryDataGenerator : IDisposable
    {
        private readonly MemoryStream valueStream;

        private BinaryDataGenerator(MemoryStream valueStream)
        {
            this.valueStream = valueStream;
        }

        public static BinaryDataGenerator New()
            => new BinaryDataGenerator(new MemoryStream());

        public BinaryData GetData()
            => BinaryData.OfValue(valueStream.ToArray());

        public BinaryDataGenerator Append<TType>(TType typeValue, IEncoder<TType> typeEncoder) where TType : class
            => Append(typeEncoder.Encode(typeValue));

        public BinaryDataGenerator Append(BinaryData data)
        {
            valueStream.Write(data.Value, 0, data.Length);

            return this;
        }

        public void Dispose()
        {
            valueStream.Dispose();
        }
    }
}
