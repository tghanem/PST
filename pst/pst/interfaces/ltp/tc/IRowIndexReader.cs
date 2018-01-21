using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader
    {
        Maybe<int> GetRowIndex(NID[] nodePath, int rowId);

        TCROWID[] GetAllRowIds(NID[] nodePath);

        TCOLDESC[] GetColumns(NID[] nodePath);
    }
}
