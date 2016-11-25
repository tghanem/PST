using pst.core;

namespace pst.interfaces.btree
{
    interface IBTreeKeyFinder<TKey, TReferenceKey> where TKey : class
    {
         Maybe<TKey> Find(TReferenceKey key);
    }
}