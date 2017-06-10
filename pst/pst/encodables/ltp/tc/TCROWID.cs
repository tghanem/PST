using pst.utilities;

namespace pst.encodables.ltp.tc
{
    class TCROWID
    {
        ///4
        public BinaryData RowId { get; }

        ///4
        public int RowIndex { get; }

        public TCROWID(BinaryData rowId, int rowIndex)
        {
            RowId = rowId;
            RowIndex = rowIndex;
        }
    }
}
