using System;
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
            => new BinaryData(new[] {value});

        public static BinaryData OfValue(byte[] value)
            => new BinaryData(value);

        public BinaryData Take(int count)
            => new BinaryData(Value.Take(count).ToArray());

        public BinaryData Pad(int count)
            => new BinaryData(Value.Concat(new byte[count]).ToArray());

        public int Length => Value.Length;
    }
}
