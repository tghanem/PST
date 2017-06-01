using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    class TableRow
    {
        public NID RowId { get; }

        public IReadOnlyDictionary<int, BinaryData> Values { get; }

        public TableRow(NID rowId, IReadOnlyDictionary<int, BinaryData> values)
        {
            RowId = rowId;
            Values = values;
        }
    }

    interface IRowMatrixReader<TRowId>
    {
        Maybe<TableRow> GetRow(
            IMapper<NID, SLEntry> nidToSLEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            TRowId rowId);
    }
}
