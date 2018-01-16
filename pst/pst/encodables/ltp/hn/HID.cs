using pst.utilities;

namespace pst.encodables.ltp.hn
{
    class HID
    {
        public static readonly HID Zero = new HID(0, 0, 0);

        ///5 bits
        public int Type { get; }

        ///11 bits
        public int Index { get; }

        ///16 bits
        public int BlockIndex { get; }

        public HID(int type, int index, int blockIndex)
        {
            Type = type;
            Index = index;
            BlockIndex = blockIndex;
        }

        public static HID OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var value = parser.TakeAndSkip(4).ToInt32();

            return
                new HID(
                    value & 0x0000001F,
                    (value >> 5) & 0x000003FF,
                    (value >> 16) & 0x0000FFFF);
        }

        public int Value => BlockIndex << 16 | Index << 5 | Type;

        public bool IsZero => Value == 0;

        public override bool Equals(object obj)
        {
            var hid = obj as HID;

            return hid?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"0x{Value:x}".ToLower();
        }
    }
}
