using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb.btree;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader<TRowId>
    {
        Maybe<TCROWID> GetRowId(LBBTEntry blockEntry, TRowId rowId);

        TCROWID[] GetAllRowIds(LBBTEntry blockEntry);
    }
}
