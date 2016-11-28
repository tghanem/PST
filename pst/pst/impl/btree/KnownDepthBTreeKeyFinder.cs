using pst.interfaces.btree;
using pst.core;
using pst.interfaces;

namespace pst.impl.btree
{
    class KnownDepthBTreeKeyFinder<TNode, TNodeReference, TIntermediateKey, TLeafKey, TReferenceKey> : IBTreeKeyFinder<TLeafKey, TReferenceKey>
        where TIntermediateKey : class
        where TNodeReference : class
        where TReferenceKey : class
        where TLeafKey : class
        where TNode : class
    {
        private readonly IBTreeNodeKeyLocator<TNode, TIntermediateKey, TReferenceKey> intermediateEntryLocator;
        private readonly IBTreeNodeKeyLocator<TNode, TLeafKey, TReferenceKey> leafEntryLocator;
        private readonly IBTreeNodeLoader<TNode, TNodeReference> intermediateNodeLoader;
        private readonly TNodeReference rootNodeReference;
        private readonly IExtractor<TIntermediateKey, TNodeReference> nodeReferenceFromIntermediateEntry;
        private readonly int treeDepth;

        public KnownDepthBTreeKeyFinder(
            IBTreeNodeKeyLocator<TNode, TIntermediateKey, TReferenceKey> intermediateEntryLocator,
            IBTreeNodeKeyLocator<TNode, TLeafKey, TReferenceKey> leafEntryLocator,
            IBTreeNodeLoader<TNode, TNodeReference> intermediateNodeLoader,
            TNodeReference rootNodeReference,
            IExtractor<TIntermediateKey, TNodeReference> nodeReferenceFromIntermediateEntry,
            int treeDepth)
        {
            this.intermediateEntryLocator = intermediateEntryLocator;
            this.leafEntryLocator = leafEntryLocator;
            this.intermediateNodeLoader = intermediateNodeLoader;
            this.rootNodeReference = rootNodeReference;
            this.nodeReferenceFromIntermediateEntry = nodeReferenceFromIntermediateEntry;
            this.treeDepth = treeDepth;
        }

        public Maybe<TLeafKey> Find(TReferenceKey key)
        {
            return Find(key, rootNodeReference, treeDepth);
        }

        private Maybe<TLeafKey> Find(TReferenceKey key, TNodeReference nodeReference, int currentDepth)
        {
            var node = intermediateNodeLoader.LoadNode(nodeReference);

            if (node.HasNoValue)
            {
                return Maybe<TLeafKey>.NoValue<TLeafKey>();
            }

            if (currentDepth > 0)
            {
                var entry = intermediateEntryLocator.FindKey(node.Value, key);

                if (entry.HasNoValue)
                {
                    return Maybe<TLeafKey>.NoValue<TLeafKey>();
                }

                return Find(key, nodeReferenceFromIntermediateEntry.Extract(entry.Value), currentDepth - 1);
            }
            else
            {
                return leafEntryLocator.FindKey(node.Value, key);
            }
        }
    }
}
