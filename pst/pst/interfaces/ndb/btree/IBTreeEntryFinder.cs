using pst.core;

namespace pst.interfaces.ndb.btree
{
    interface IBTreeEntryFinder<TKey, TEntry> where TEntry : class
    {
         Maybe<TEntry> Find(TKey key);
    }
}