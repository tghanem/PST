using pst.interfaces.ltp.hn;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    interface IPropertiesFromTableContextRowLoader
    {
        Dictionary<PropertyId, PropertyValue> Load(
            HeapOnNode heapOnNode,
            TableRow tableRow);
    }
}
