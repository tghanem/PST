using pst.encodables.ltp.tc;
using pst.interfaces.ltp.hn;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexLoader
    {
        TCROWID[] Load(HeapOnNode heapOnNode);
    }
}
