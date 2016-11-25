using pst.core;
using pst.interfaces.btree;
using System;

namespace pst.impl.btree
{
    class BTreeKeyFinder<TNode, TNodeReference, TIntermediateKey, TLeafKey, TReferenceKey> : IBTreeKeyFinder<TLeafKey, TReferenceKey>
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
        private readonly Func<TIntermediateKey, TNodeReference> intermediateEntryToNodeReference;
        private readonly Func<TNode, int> getNodeLevel;

        public BTreeKeyFinder(
            IBTreeNodeKeyLocator<TNode, TIntermediateKey, TReferenceKey> intermediateEntryLocator,
            IBTreeNodeKeyLocator<TNode, TLeafKey, TReferenceKey> leafEntryLocator,
            IBTreeNodeLoader<TNode, TNodeReference> intermediateNodeLoader,
            TNodeReference rootNodeReference,
            Func<TIntermediateKey, TNodeReference> intermediateEntryToNodeReference,
            Func<TNode, int> getNodeLevel)
        {
            this.intermediateEntryLocator = intermediateEntryLocator;
            this.leafEntryLocator = leafEntryLocator;
            this.intermediateNodeLoader = intermediateNodeLoader;
            this.rootNodeReference = rootNodeReference;
            this.intermediateEntryToNodeReference = intermediateEntryToNodeReference;
            this.getNodeLevel = getNodeLevel;
        }

        public Maybe<TLeafKey> Find(TReferenceKey key)
        {
            return Find(key, rootNodeReference);
        }

        private Maybe<TLeafKey> Find(TReferenceKey key, TNodeReference nodeReference)
        {
            var node = intermediateNodeLoader.LoadNode(nodeReference);

            if (node.HasNoValue)
            {
                return Maybe<TLeafKey>.NoValue<TLeafKey>();
            }

            if (getNodeLevel(node.Value) > 0)
            {
                var entry = intermediateEntryLocator.FindKey(node.Value, key);

                if (entry.HasNoValue)
                {
                    return Maybe<TLeafKey>.NoValue<TLeafKey>();
                }

                return Find(key, intermediateEntryToNodeReference(entry.Value));
            }
            else
            {
                return leafEntryLocator.FindKey(node.Value, key);
            }
        }
    }
}
