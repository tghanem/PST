using pst.encodables.ltp.bth;
using pst.interfaces.ltp.hn;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapLeafKeysEnumerator
    {
        DataRecord[] Enumerate(HeapOnNode heapOnNode);
    }
}
