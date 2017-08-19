using System;
using pst.core;

namespace pst.interfaces.btree
{
    public interface IBTreeEntryFinder<TEntryKey, TEntry, TNodeReference>
        where TEntry : class
        where TEntryKey : IComparable<TEntryKey>
    {
        Maybe<TEntry> Find(TEntryKey key, TNodeReference rootNodeReference);
    }
}