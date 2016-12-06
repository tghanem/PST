using pst.encodables.ltp.hn;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeItemsLoader
    {
        IDictionary<HID, BinaryData> Load(int pageIndex, HNPAGEMAP pageMap, BinaryData pageData);
    }
}
