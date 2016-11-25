using pst.core;

namespace pst.interfaces.ndb.btree
{
    interface IBTreeNodeKeysComparer<TKey, TReferenceKey>
        where TReferenceKey : class
        where TKey : class
    {
        Maybe<TKey> GetMatchingKey(TKey[] keys, TReferenceKey key);
    }
}