using pst.encodables.ndb;
using System;

namespace pst.interfaces.btree
{
    interface IBTreeGenerator<TKey, TValue> where TKey : IComparable<TKey>
    {
        BREF Generate(Tuple<TKey, TValue>[] values);
    }
}