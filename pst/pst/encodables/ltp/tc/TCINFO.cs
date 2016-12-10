using pst.encodables.ltp.hn;
using pst.utilities;

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
    }
}
