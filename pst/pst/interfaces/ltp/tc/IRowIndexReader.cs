using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader<TRowId>
    {
        Maybe<TCROWID> GetRowId(BID blockId, TRowId rowId);

        TCROWID[] GetAllRowIds(BID blockId);
    }
}
