using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pst.utilities
{
    public class BinaryData
    {
        public byte[] Value { get; }

        private BinaryData(byte[] value)
        {
            Value = value;
        }

        public static BinaryData OfSize(int size)
            => new BinaryData(new byte[size]);

        public static BinaryData OfSize(int size, byte value)
        {
            var newArray = new byte[size];

            for (var i = 0; i < newArray.Length; i++)
            {
                newArray[i] = value;
            }

            return new BinaryData(newArray);
        }

        public static BinaryData Empty()
            => new BinaryData(new byte[0]);

        public static BinaryData From(int value)
            => new BinaryData(BitConverter.GetBytes(value));

        public static BinaryData From(byte value)
            => new BinaryData(new[] { value });

        public static BinaryData OfValue(byte[] value)
            => new BinaryData(value);

        public static implicit operator byte[] (BinaryData data)
            => data.Value;

        public BinaryData Take(int count)
            => new BinaryData(Value.Take(count).ToArray());

        public BinaryData Take(int offset, int count)
            => new BinaryData(Value.Skip(offset).Take(count).ToArray());

        public BinaryData Pad(int count)
            => new BinaryData(Value.Concat(new byte[count]).ToArray());

        public BinaryData Reverse()
            => new BinaryData(Value.Reverse().ToArray());

        public string ToUnicode()
        {
            return Encoding.Unicode.GetString(Value);
        }

        public string ToAnsi()
        {
            return Encoding.ASCII.GetString(Value);
        }

        public int ToInt32()
        {
            if (Value.Length < 4)
            {
                var paddedData = Pad(4 - Value.Length);

                return BitConverter.ToInt32(paddedData.Value, 0);
            }
            else if (Value.Length == 4)
            {
                return BitConverter.ToInt32(Value, 0);
            }
            else
            {
                throw new InvalidOperationException("Invalid data length");
            }
        }

        public bool HasFlag(int value)
        {
            return (value & ToInt32()) == value;
        }

        public bool ToBoolean()
        {
            return BitConverter.ToBoolean(Value, 0);
        }

        public long ToInt64()
        {
            return BitConverter.ToInt64(Value, 0);
        }

        public BinaryData[] Slice(int itemLength)
        {
            var numberOfItems =
                (int)
                Math.Floor((double)Length / itemLength);

            var slices = new List<BinaryData>();

            for (int i = 0; i < numberOfItems; i++)
            {
                slices.Add(Take(i * itemLength, itemLength));
            }

            return slices.ToArray();
        }

        public override bool Equals(object obj)
        {
            var data = obj as BinaryData;

            return data?.Value.SequenceEqual(Value) ?? false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int Length => Value.Length;
    }
}
