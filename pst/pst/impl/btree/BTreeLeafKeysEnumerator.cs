using pst.interfaces.btree;
using pst.interfaces;
using System.Collections.Generic;
using pst.interfaces.io;
using System;

namespace pst.impl.btree
{
    class BTreeLeafKeysEnumerator<TNode, TNodeReference, TIntermediateKey, TLeafKey> : IBTreeLeafKeysEnumerator<TLeafKey, TNodeReference>
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

        public BTreeLeafKeysEnumerator(
            IExtractor<TIntermediateKey, TNodeReference> nodeReferenceFromIntermediateKeyExtractor,
            IExtractor<TNode, TIntermediateKey[]> intermediateKeysExtractor,
            IExtractor<TNode, TLeafKey[]> leafKeysExtractor,
            IExtractor<TNode, int> nodeLevelFromNodeExtractor,
            IBTreeNodeLoader<TNode, TNodeReference> nodeLoader)
        {
            this.nodeReferenceFromIntermediateKeyExtractor = nodeReferenceFromIntermediateKeyExtractor;
            this.intermediateKeysExtractor = intermediateKeysExtractor;
            this.leafKeysExtractor = leafKeysExtractor;
            this.nodeLevelFromNodeExtractor = nodeLevelFromNodeExtractor;
            this.nodeLoader = nodeLoader;
        }

        public TLeafKey[] Enumerate(IDataBlockReader<TNodeReference> reader, TNodeReference rootNodeReference)
        {
            var leafKeys = new List<TLeafKey>();

            EnumerateAndAdd(
                reader,
                rootNodeReference,
                leafKeys);

            return leafKeys.ToArray();
        }

        private void EnumerateAndAdd(IDataBlockReader<TNodeReference> reader, TNodeReference nodeReference, List<TLeafKey> leafKeys)
        {
            var node =
                nodeLoader.LoadNode(reader, nodeReference);

            if (node.HasNoValue)
            {
                return;
            }

            if (nodeLevelFromNodeExtractor.Extract(node.Value) > 0)
            {
                var intermediateKeys =
                    intermediateKeysExtractor.Extract(node.Value);

                foreach (var key in intermediateKeys)
                {
                    EnumerateAndAdd(
                        reader,
                        nodeReferenceFromIntermediateKeyExtractor.Extract(key),
                        leafKeys);
                }
            }
            else
            {
                leafKeys.AddRange(
                    leafKeysExtractor.Extract(node.Value));
            }
        }
    }
}
