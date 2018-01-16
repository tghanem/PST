using System.Collections.Generic;
using pst.encodables.ltp.tc;
using pst.utilities;

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
}