using pst.interfaces.ltp.hn;
using System;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapGenerator<TKey, TValue> where TKey : IComparable<TKey>
    {
        void Generate(
            int keySize,
            int valueSize,
            Tuple<TKey, TValue>[] dataRecords,
            IHeapOnNodeGenerator hnGenerator);
    }
}