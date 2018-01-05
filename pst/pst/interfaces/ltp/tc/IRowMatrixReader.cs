using pst.core;
using pst.encodables.ndb;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    class TableRow
    {
        public BinaryData RowId { get; }

        public IReadOnlyDictionary<int, BinaryData> Values { get; }

        public TableRow(BinaryData rowId, IReadOnlyDictionary<int, BinaryData> values)
        {
            RowId = rowId;
            Values = values;
        }
    }

    interface IRowMatrixReader<TRowId>
    {
        Maybe<TableRow> GetRow(NID[] nodePath, TRowId rowId);
    }
}
