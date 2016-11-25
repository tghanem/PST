using pst.core;

namespace pst.interfaces.btree
{
    interface IBTreeNodeKeyLocator<TNode, TKey, TReferenceKey>
        where TReferenceKey : class
        where TNode : class
        where TKey : class
    {
        Maybe<TKey> FindKey(TNode node, TReferenceKey key);
    }
}