﻿using System;

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

        public int Length => Value.Length;
    }
}
