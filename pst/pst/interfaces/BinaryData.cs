using System;
using System.Linq;

namespace pst.interfaces
{
    public class BinaryData
    {
        public byte[] Value { get; }

        public BinaryData(byte[] value)
        {
            Value = value;
        }

        public BinaryData TakeAsInt32AndSkip(int count, ref int value)
        {
            var binaryValue = (BinaryData) null;

            var remainingData =
                TakeAndSkip(
                    count,
                    ref binaryValue);

            value = BitConverter.ToInt32(binaryValue.Value, 0);

            return remainingData;
        }

        public BinaryData TakeAndSkip(int count, ref BinaryData value)
        {
            value =
                new BinaryData(
                    Value
                    .Take(count)
                    .ToArray());

            return
                new BinaryData(
                    Value
                    .Skip(count)
                    .ToArray());
        }
    }
}
