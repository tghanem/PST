using System;
using pst.interfaces;
using System.IO;

namespace pst.utilities
{
    class BinaryDataGenerator
    {
        private readonly MemoryStream valueStream;

        private BinaryDataGenerator(MemoryStream valueStream)
        {
            this.valueStream = valueStream;
        }

        public static BinaryDataGenerator New()
            => new BinaryDataGenerator(new MemoryStream());

        public long DataSize => valueStream.Length;

        public BinaryData GetData()
            => BinaryData.OfValue(valueStream.ToArray());

        public BinaryDataGenerator Append<TType>(TType typeValue, IEncoder<TType> typeEncoder)
            => Append(typeEncoder.Encode(typeValue));

        public BinaryDataGenerator Append<TType>(TType typeValue, IEncoder<TType> typeEncoder, int countOfEncodedDataToAppend)
            => Append(typeEncoder.Encode(typeValue).Take(countOfEncodedDataToAppend));

        public BinaryDataGenerator Append(int data)
            => Append(BitConverter.GetBytes(data));

        public BinaryDataGenerator Append(short data)
            => Append(BitConverter.GetBytes(data));

        public BinaryDataGenerator Append(BinaryData data)
            => Append(data.Value);

        public BinaryDataGenerator Append(BinaryData[] data)
        {
            Array.ForEach(data, d => Append(d.Value));
            return this;
        }

        public BinaryDataGenerator FillTo(int size)
        {
            var data = new byte[Math.Abs(size - valueStream.Length)];
            valueStream.Write(data, 0, data.Length);
            return this;
        }

        public BinaryDataGenerator Append(byte data)
        {
            valueStream.WriteByte(data);
            return this;
        }

        public BinaryDataGenerator Append(byte[] data)
        {
            valueStream.Write(data, 0, data.Length);
            return this;
        }

        public BinaryDataGenerator WriteTo(Stream stream)
        {
            valueStream.CopyTo(stream);
            return this;
        }
    }
}
