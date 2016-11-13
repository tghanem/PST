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

        public BinaryDataParser TakeAsInt32AndSkip(int count, ref int value)
        {
            var binaryValue = (BinaryData) null;

            var remainingData =
                TakeAndSkip(count, ref binaryValue);

            value = BitConverter.ToInt32(binaryValue.Value, 0);

            return this;
        }

        public BinaryDataParser TakeAndSkip(int count, ref BinaryData value)
        {
            if (count > valueStream.Position + valueStream.Length)
                throw new ArgumentException("Value stream does not have the required number of bytes");

            var buffer = new byte[count];

            valueStream.Read(buffer, 0, count);

            value = BinaryData.OfValue(buffer);

            return this;
        }

        public void Dispose()
        {
            valueStream.Dispose();
        }
    }
}
