using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowMatrixReader
    {
        Maybe<TableRow> GetRow(NID[] nodePath, TCROWID rowId);
    }
}
