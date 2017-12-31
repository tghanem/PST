using pst.core;
using System;

namespace pst.interfaces.btree
{
    interface IBTreeEntryFinder<TEntryKey, TEntry, TNodeReference> where TEntryKey : IComparable<TEntryKey>
    {
        Maybe<TEntry> Find(TEntryKey key, TNodeReference rootNodeReference);
    }
}