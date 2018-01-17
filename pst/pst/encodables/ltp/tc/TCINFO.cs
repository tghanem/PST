using pst.encodables.ltp.hn;
using pst.impl;
using pst.utilities;
using System.Linq;

namespace pst.encodables.ltp.tc
{
    class TCINFO
    {
        ///1
        public int Type { get; }

        ///1
        public int NumberOfColumns { get; }

        ///4 * (2)
        public int[] GroupsOffsets { get; }

        ///4
        public HID RowIndex { get; }

        ///4 (HNID)
        public BinaryData RowMatrix { get; }

        ///4
        public BinaryData Deprecated { get; }

        ///NumberOfColumns * 16
        public TCOLDESC[] ColumnDescriptors { get; }

        public TCINFO(
            int type,
            int numberOfColumns,
            int[] groupsOffsets,
            HID rowIndex,
            BinaryData rowMatrix,
            BinaryData deprecated,
            TCOLDESC[] columnDescriptors)
        {
            Type = type;
            NumberOfColumns = numberOfColumns;
            GroupsOffsets = groupsOffsets;
            RowIndex = rowIndex;
            RowMatrix = rowMatrix;
            Deprecated = deprecated;
            ColumnDescriptors = columnDescriptors;
        }

        public static TCINFO OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var type = parser.TakeAndSkip(1).ToInt32();
            var numberOfColumns = parser.TakeAndSkip(1).ToInt32();
            var groupsOffsets = parser.Slice(4, 2).Select(s => s.ToInt32()).ToArray();
            var rowIndex = HID.OfValue(parser.TakeAndSkip(4));
            var rowMatrix = parser.TakeAndSkip(4);
            var deprecated = parser.TakeAndSkip(4);
            var columnDescriptors = parser.TakeAndSkip(numberOfColumns, 8, new FuncBasedDecoder<TCOLDESC>(TCOLDESC.OfValue));

            return
                new TCINFO(
                    type,
                    numberOfColumns,
                    groupsOffsets,
                    rowIndex,
                    rowMatrix,
                    deprecated,
                    columnDescriptors);
        }
    }
}
