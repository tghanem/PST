using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.utilities;

namespace pst.interfaces.ltp.tc
{
    class TableRow
    {
        public BinaryData[] ColumnValues { get; }

        public TableRow(BinaryData[] columnValues)
        {
            ColumnValues = columnValues;
        }
    }

    interface ITableContextLoader
    {
        TableRow[] Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
