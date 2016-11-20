using pst.core;
using pst.interfaces;
using System;
using System.Linq;

namespace pst.impl
{
    class LeafPageEntriesComparer<TKey, TEntry> : IBTreePageEntriesComparer<TKey, TEntry>
        where TKey : IEquatable<TKey>
        where TEntry : class
    {
        private readonly Func<TEntry, TKey> entryToKey;

        public LeafPageEntriesComparer(Func<TEntry, TKey> entryToKey)
        {
            this.entryToKey = entryToKey;
        }

        public Maybe<TEntry> GetMatchingEntry(TEntry[] entries, TKey key)
        {
            return
                entries
                .FirstOrDefault(e => entryToKey(e).Equals(key));
        }
    }
}
