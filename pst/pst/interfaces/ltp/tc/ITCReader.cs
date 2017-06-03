using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface ITCReader<TRowId>
    {
        Maybe<TableRow> GetRow(NID nodeId, TRowId rowId);

        TCROWID[] GetAllRowIds(NID nodeId);
    }
}
