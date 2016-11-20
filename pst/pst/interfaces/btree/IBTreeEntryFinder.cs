using pst.core;

namespace pst.interfaces
{
    interface IBTreeEntryFinder<TKey, TEntry> where TEntry : class
    {
         Maybe<TEntry> Find(TKey key);
    }
}