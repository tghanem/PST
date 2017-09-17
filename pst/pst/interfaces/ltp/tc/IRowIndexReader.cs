using pst.core;
using pst.encodables.ltp.tc;
using pst.interfaces.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader<TRowId>
    {
        Maybe<TCROWID> GetRowId(NodePath nodePath, TRowId rowId);

        TCROWID[] GetAllRowIds(NodePath nodePath);
    }
}
