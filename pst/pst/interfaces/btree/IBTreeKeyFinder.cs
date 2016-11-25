using pst.core;

namespace pst.interfaces.ndb.btree
{
    interface IBTreeKeyFinder<TKey, TReferenceKey> where TKey : class
    {
         Maybe<TKey> Find(TReferenceKey key);
    }
}