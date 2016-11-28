using pst.interfaces.btree;
using pst.core;
using pst.interfaces;

namespace pst.impl.btree
{
    class BTreeNodeKeyLocator<TNode, TKey, TReferenceKey> : IBTreeNodeKeyLocator<TNode, TKey, TReferenceKey>
        where TReferenceKey : class
        where TNode : class
        where TKey : class
    {
        private readonly IBTreeNodeKeysComparer<TKey, TReferenceKey> nodeKeysComparer;
        private readonly IExtractor<TNode, TKey[]> nodeKeysExtractor;

        public BTreeNodeKeyLocator(
            IBTreeNodeKeysComparer<TKey, TReferenceKey> pageEntriesComparer,
            IExtractor<TNode, TKey[]> nodeKeysExtractor)
        {
            this.nodeKeysComparer = pageEntriesComparer;
            this.nodeKeysExtractor = nodeKeysExtractor;
        }

        public Maybe<TKey> FindKey(TNode node, TReferenceKey key)
        {
            return
                nodeKeysComparer
                    .GetMatchingKey(
                        nodeKeysExtractor.Extract(node),
                        key);
        }
    }
}
