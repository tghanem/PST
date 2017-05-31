using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader<TRowId>
    {
        Maybe<TCROWID> GetRowId(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            TRowId rowId);

        TCROWID[] GetAllRowIds(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
