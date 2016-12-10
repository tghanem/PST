using pst.encodables.ndb;

namespace pst.encodables.ltp.tc
{
    class TCROWID
    {
        ///4
        public NID RowId { get; }

        ///4
        public int RowIndex { get; }

        public TCROWID(NID rowId, int rowIndex)
        {
            RowId = rowId;
            RowIndex = rowIndex;
        }
    }
}
