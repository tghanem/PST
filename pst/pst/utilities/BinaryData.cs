using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.utilities
{
    class BinaryData
    {
        public byte[] Value { get; }

        private BinaryData(byte[] value)
        {
            Value = value;
        }

        public static BinaryData Empty()
            => new BinaryData(new byte[0]);

        public static BinaryData From(int value)
            => new BinaryData(BitConverter.GetBytes(value));

        public static BinaryData From(byte value)
            => new BinaryData(new[] { value });

        public static BinaryData OfValue(byte[] value)
            => new BinaryData(value);

        public BinaryData Take(int count)
            => new BinaryData(Value.Take(count).ToArray());

        public BinaryData TakeAt(int offset, int count)
            => new BinaryData(Value.Skip(offset).Take(count).ToArray());

        public BinaryData Pad(int count)
            => new BinaryData(Value.Concat(new byte[count]).ToArray());

        public BinaryData[] Slice(int itemLength)
        {
            var numberOfItems =
                (int)
                Math.Floor((double)Length / itemLength);

            var slices = new List<BinaryData>();

            for (int i = 0; i < numberOfItems; i++)
            {
                slices.Add(TakeAt(i * itemLength, itemLength));
            }

            return slices.ToArray();
        }

        public int[] ToBits()
        {
            var bits = new List<int>();

            foreach (var b in Value)
            {
                bits.Add(b & 0x01);
                bits.Add((b & 0x02) >> 1);
                bits.Add((b & 0x04) >> 2);
                bits.Add((b & 0x08) >> 3);
                bits.Add((b & 0x10) >> 4);
                bits.Add((b & 0x20) >> 5);
                bits.Add((b & 0x40) >> 6);
                bits.Add((b & 0x80) >> 7);
            }

            return bits.ToArray();
        }

        public int Length => Value.Length;
    }
}
