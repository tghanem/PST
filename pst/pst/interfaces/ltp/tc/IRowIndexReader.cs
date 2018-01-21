using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader
    {
        Maybe<TCROWID> GetRowId(NID[] nodePath, int rowId);

        TCROWID[] GetAllRowIds(NID[] nodePath);
    }
}
