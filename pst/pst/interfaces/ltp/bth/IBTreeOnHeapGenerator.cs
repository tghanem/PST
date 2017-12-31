using pst.encodables.ltp.hn;
using pst.interfaces.ltp.hn;
using System;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapGenerator<TKey, TValue> where TKey : IComparable<TKey>
    {
        HID Generate(Tuple<TKey, TValue>[] dataRecords, IHeapOnNodeGenerator hnGenerator);
    }
}