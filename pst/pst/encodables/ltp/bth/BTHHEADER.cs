using pst.encodables.ltp.hn;
using pst.utilities;

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

        public static BTHHEADER OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BTHHEADER(
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    HID.OfValue(parser.TakeAndSkip(4)));
        }
    }
}
