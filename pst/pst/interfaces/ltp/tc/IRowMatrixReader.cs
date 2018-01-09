using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    class TableRow
    {
        public TCROWID RowId { get; }

        public IReadOnlyDictionary<int, BinaryData> Values { get; }

        public TableRow(TCROWID rowId, IReadOnlyDictionary<int, BinaryData> values)
        {
            RowId = rowId;
            Values = values;
        }
    }

    interface IRowMatrixReader
    {
        Maybe<TableRow> GetRow(NID[] nodePath, TCROWID rowId);
    }
}
