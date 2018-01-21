using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    class TableRow
    {
        public IReadOnlyDictionary<int, BinaryData> Values { get; }

        public TableRow(IReadOnlyDictionary<int, BinaryData> values)
        {
            Values = values;
        }
    }
}