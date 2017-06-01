using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using System.Collections.Generic;

namespace pst.impl.btree
{
    class BTreeLeafKeysEnumerator<TNode, TNodeReference, TIntermediateKey, TLeafKey>
        : IBTreeLeafKeysEnumerator<TLeafKey, TNodeReference>

        where TIntermediateKey : class
        where TNodeReference : class
        where TLeafKey : class
        where TNode : class
    {
        private readonly IExtractor<TIntermediateKey, TNodeReference> nodeReferenceFromIntermediateKeyExtractor;
        private readonly IExtractor<TNode, TIntermediateKey[]> intermediateKeysExtractor;
        private readonly IExtractor<TNode, TLeafKey[]> leafKeysExtractor;
        private readonly IExtractor<TNode, int> nodeLevelFromNodeExtractor;

        private readonly IBTreeNodeLoader<TNode, TNodeReference> nodeLoader;

        private readonly IDataBlockReader<TNodeReference> nodeDataBlockReader;

        public BTreeLeafKeysEnumerator(
            IExtractor<TIntermediateKey, TNodeReference> nodeReferenceFromIntermediateKeyExtractor,
            IExtractor<TNode, TIntermediateKey[]> intermediateKeysExtractor,
            IExtractor<TNode, TLeafKey[]> leafKeysExtractor,
            IExtractor<TNode, int> nodeLevelFromNodeExtractor,
            IBTreeNodeLoader<TNode, TNodeReference> nodeLoader,
            IDataBlockReader<TNodeReference> nodeDataBlockReader)
        {
            this.nodeReferenceFromIntermediateKeyExtractor = nodeReferenceFromIntermediateKeyExtractor;
            this.intermediateKeysExtractor = intermediateKeysExtractor;
            this.leafKeysExtractor = leafKeysExtractor;
            this.nodeLevelFromNodeExtractor = nodeLevelFromNodeExtractor;
            this.nodeLoader = nodeLoader;
            this.nodeDataBlockReader = nodeDataBlockReader;
        }

        public TLeafKey[] Enumerate(TNodeReference rootNodeReference)
        {
            var leafKeys = new List<TLeafKey>();

            EnumerateAndAdd(
                rootNodeReference,
                leafKeys);

            return leafKeys.ToArray();
        }

        private void EnumerateAndAdd(TNodeReference nodeReference, List<TLeafKey> leafKeys)
        {
            var node =
                nodeLoader.LoadNode(nodeReference);

            if (nodeLevelFromNodeExtractor.Extract(node) > 0)
            {
                var intermediateKeys =
                    intermediateKeysExtractor.Extract(node);

                foreach (var key in intermediateKeys)
                {
                    EnumerateAndAdd(
                        nodeReferenceFromIntermediateKeyExtractor.Extract(key),
                        leafKeys);
                }
            }
            else
            {
                leafKeys.AddRange(leafKeysExtractor.Extract(node));
            }
        }
    }
}
