using pst.interfaces.btree;
using pst.core;
using System;

namespace pst.impl.btree
{
    class BTreeNodeKeyLocator<TNode, TKey, TReferenceKey> : IBTreeNodeKeyLocator<TNode, TKey, TReferenceKey>
        where TReferenceKey : class
        where TNode : class
        where TKey : class
    {
        private readonly IBTreeNodeKeysComparer<TKey, TReferenceKey> nodeKeysComparer;
        private readonly Func<TNode, TKey[]> getNodeKeys;

        public BTreeNodeKeyLocator(
            IBTreeNodeKeysComparer<TKey, TReferenceKey> pageEntriesComparer,
            Func<TNode, TKey[]> getNodeKeys)
        {
            this.nodeKeysComparer = pageEntriesComparer;
            this.getNodeKeys = getNodeKeys;
        }

        public Maybe<TKey> FindKey(TNode node, TReferenceKey key)
        {
            return
                nodeKeysComparer
                    .GetMatchingKey(
                        getNodeKeys(node),
                        key);
        }
    }
}
