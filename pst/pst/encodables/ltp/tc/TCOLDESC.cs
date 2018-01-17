using pst.utilities;

namespace pst.encodables.ltp.tc
{
    class TCOLDESC
    {
        ///4
        public int Tag { get; }

        ///2
        public int DataOffset { get; }

        ///1
        public int DataSize { get; }

        ///1
        public int CellExistenceBitmapIndex { get; }

        public TCOLDESC(int tag, int dataOffset, int dataSize, int cellExistenceBitmapIndex)
        {
            Tag = tag;
            DataOffset = dataOffset;
            DataSize = dataSize;
            CellExistenceBitmapIndex = cellExistenceBitmapIndex;
        }

        public static TCOLDESC OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new TCOLDESC(
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32());
        }
    }
}
