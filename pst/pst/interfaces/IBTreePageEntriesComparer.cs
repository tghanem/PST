namespace pst.interfaces
{
    interface IBTreePageEntriesComparer<TKey, TEntry>
    {
        TEntry GetMatchingEntry(TEntry[] entries, TKey key);
    }
}
