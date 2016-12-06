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
    }
}
