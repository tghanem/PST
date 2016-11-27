using pst.encodables.ltp.hn;

namespace pst.encodables.ltp.bth
{
    class BTHHEADER
    {
        ///1
        public int Type { get; }

        ///1
        public int Key { get; }

        ///1
        public int SizeOfDataValue { get; }

        ///1
        public int IndexDepth { get; }

        ///4
        public HID Root { get; }

        public BTHHEADER(int type, int key, int sizeOfDataValue, int indexDepth, HID root)
        {
            Type = type;
            Key = key;
            SizeOfDataValue = sizeOfDataValue;
            IndexDepth = indexDepth;
            Root = root;
        }
    }
}
