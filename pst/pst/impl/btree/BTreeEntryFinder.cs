using pst.core;
using pst.interfaces;
using pst.interfaces.btree;
using System;

namespace pst.impl.btree
{
    class BTreeEntryFinder<TEntryKey, TLeafEntry, TIntermediateEntry, TNodeReference, TNode> : IBTreeEntryFinder<TEntryKey, TLeafEntry, TNodeReference> where TEntryKey : IComparable<TEntryKey>
    {
        private readonly IExtractor<TLeafEntry, TEntryKey> entryKeyFromLeafEntryExtractor;
        private readonly IExtractor<TIntermediateEntry, TEntryKey> entryKeyFromIntermediateEntryExtractor;
        private readonly IExtractor<TIntermediateEntry, TNodeReference> nodeReferenceFromIntermediateEntryExtractor;
        private readonly IExtractor<TNode, TIntermediateEntry[]> intermediateEntriesFromNodeExtractor;
        private readonly IExtractor<TNode, TLeafEntry[]> leafEntriesFromNodeExtractor;
        private readonly IExtractor<TNode, int> nodeLevelFromNodeExtractor;

        private readonly IBTreeNodeLoader<TNode, TNodeReference> nodeLoader;

        public BTreeEntryFinder(
            IExtractor<TLeafEntry, TEntryKey> entryKeyFromLeafEntryExtractor,
            IExtractor<TIntermediateEntry, TEntryKey> entryKeyFromIntermediateEntryExtractor,
            IExtractor<TIntermediateEntry, TNodeReference> nodeReferenceFromIntermediateEntryExtractor,
            IExtractor<TNode, TIntermediateEntry[]> intermediateEntriesFromNodeExtractor,
            IExtractor<TNode, TLeafEntry[]> leafEntriesFromNodeExtractor,
            IExtractor<TNode, int> nodeLevelFromNodeExtractor,
            IBTreeNodeLoader<TNode, TNodeReference> nodeLoader)
        {
            this.entryKeyFromLeafEntryExtractor = entryKeyFromLeafEntryExtractor;
            this.entryKeyFromIntermediateEntryExtractor = entryKeyFromIntermediateEntryExtractor;
            this.nodeReferenceFromIntermediateEntryExtractor = nodeReferenceFromIntermediateEntryExtractor;
            this.intermediateEntriesFromNodeExtractor = intermediateEntriesFromNodeExtractor;
            this.leafEntriesFromNodeExtractor = leafEntriesFromNodeExtractor;
            this.nodeLevelFromNodeExtractor = nodeLevelFromNodeExtractor;
            this.nodeLoader = nodeLoader;
        }

        public Maybe<TLeafEntry> Find(TEntryKey key, TNodeReference rootNodeReference)
        {
            var node = nodeLoader.LoadNode(rootNodeReference);

            var level = nodeLevelFromNodeExtractor.Extract(node);

            if (level == 0)
            {
                var leafEntries = leafEntriesFromNodeExtractor.Extract(node);

                foreach (var entry in leafEntries)
                {
                    var leafKey = entryKeyFromLeafEntryExtractor.Extract(entry);

                    if (leafKey.CompareTo(key) == 0)
                    {
                        return Maybe<TLeafEntry>.OfValue(entry);
                    }
                }

                return Maybe<TLeafEntry>.NoValue();
            }

            var intermediateEntries = intermediateEntriesFromNodeExtractor.Extract(node);

            for (var i = 0; i < intermediateEntries.Length; i++)
            {
                var intermediateKey = entryKeyFromIntermediateEntryExtractor.Extract(intermediateEntries[i]);

                if (intermediateKey.CompareTo(key) == 0 || (intermediateKey.CompareTo(key) > 0 && i == 0))
                {
                    var nodeReference = nodeReferenceFromIntermediateEntryExtractor.Extract(intermediateEntries[i]);

                    return Find(key, nodeReference);
                }

                if (intermediateKey.CompareTo(key) > 0)
                {
                    var nodeReference = nodeReferenceFromIntermediateEntryExtractor.Extract(intermediateEntries[i - 1]);

                    return Find(key, nodeReference);
                }
            }

            var lastIntermediateEntry =
                intermediateEntries[intermediateEntries.Length - 1];

            var nodeReferenceFromLastIntermediateEntry =
                nodeReferenceFromIntermediateEntryExtractor.Extract(lastIntermediateEntry);

            return Find(key, nodeReferenceFromLastIntermediateEntry);
        }
    }
}
