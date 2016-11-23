using pst.core;
using pst.interfaces.ndb.btree;
using System;
using System.Linq;

namespace pst.impl
{
    class IntermediatePageEntriesComparer<TKey, TEntry> : IBTreePageEntriesComparer<TKey, TEntry>
        where TKey : IComparable<TKey>
        where TEntry : class
    {
        private readonly Func<TEntry, TKey> entryToKey;
        
        public IntermediatePageEntriesComparer(Func<TEntry, TKey> entryToKey)
        {
            this.entryToKey = entryToKey;
        }

        public Maybe<TEntry> GetMatchingEntry(TEntry[] entries, TKey key)
        {
            return
                entries
                .FirstOrDefault(e => entryToKey(e).CompareTo(key) <= 0);
        }
    }
}
