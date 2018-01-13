using pst.encodables.ltp.tc;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    interface IRowValuesExtractor
    {
        IReadOnlyDictionary<int, BinaryData> Extract(BinaryData rowData, TCOLDESC[] columnDescriptors, int cebOffset);
    }
}
