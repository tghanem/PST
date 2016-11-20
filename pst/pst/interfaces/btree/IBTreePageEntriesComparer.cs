using pst.core;

namespace pst.interfaces
{
    interface IBTreePageEntriesComparer<TKey, TEntry> where TEntry : class
    {
        Maybe<TEntry> GetMatchingEntry(TEntry[] entries, TKey key);
    }
}