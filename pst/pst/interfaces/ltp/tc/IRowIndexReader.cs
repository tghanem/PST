using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader<TRowId>
    {
        Maybe<TCROWID> GetRowId(NID[] nodePath, TRowId rowId);

        TCROWID[] GetAllRowIds(NID[] nodePath);
    }
}
