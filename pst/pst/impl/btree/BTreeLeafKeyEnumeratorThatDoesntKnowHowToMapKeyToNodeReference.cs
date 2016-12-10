using pst.interfaces.btree;
using System.Collections.Generic;
using pst.interfaces.io;
using pst.interfaces;

namespace pst.impl.btree
{
    class BTreeLeafKeyEnumeratorThatDoesntKnowHowToMapKeyToNodeReference<TNode, TNodeReference, TIntermediateKey, TLeafKey> : IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<TLeafKey, TIntermediateKey, TNodeReference>
        where TIntermediateKey : class
        where TNodeReference : class
        where TLeafKey : class
        where TNode : class
    {
        private readonly IExtractor<TNode, TIntermediateKey[]> intermediateKeysExtractor;
        private readonly IExtractor<TNode, TLeafKey[]> leafKeysExtractor;
        private readonly IExtractor<TNode, int> nodeLevelFromNodeExtractor;
        private readonly IBTreeNodeLoader<TNode, TNodeReference> nodeLoader;

        public BTreeLeafKeyEnumeratorThatDoesntKnowHowToMapKeyToNodeReference(
            IExtractor<TNode, TIntermediateKey[]> intermediateKeysExtractor,
            IExtractor<TNode, TLeafKey[]> leafKeysExtractor,
            IExtractor<TNode, int> nodeLevelFromNodeExtractor,
            IBTreeNodeLoader<TNode, TNodeReference> nodeLoader)
        {
            this.intermediateKeysExtractor = intermediateKeysExtractor;
            this.leafKeysExtractor = leafKeysExtractor;
            this.nodeLevelFromNodeExtractor = nodeLevelFromNodeExtractor;
            this.nodeLoader = nodeLoader;
        }

        public TLeafKey[] Enumerate(
            IDataBlockReader<TNodeReference> reader,
            IReadOnlyDictionary<TIntermediateKey, TNodeReference> keyToNodeReferenceMapping,
            TNodeReference rootNodeReference)
        {
            var leafKeys = new List<TLeafKey>();

            EnumerateAndAdd(
                reader,
                keyToNodeReferenceMapping,
                rootNodeReference,
                leafKeys);

            return leafKeys.ToArray();
        }

        private void EnumerateAndAdd(IDataBlockReader<TNodeReference> reader, IReadOnlyDictionary<TIntermediateKey, TNodeReference> keyToNodeReferenceMapping, TNodeReference nodeReference, List<TLeafKey> leafKeys)
        {
            var node =
                nodeLoader.LoadNode(reader, nodeReference);

            if (nodeLevelFromNodeExtractor.Extract(node) > 0)
            {
                var intermediateKeys =
                    intermediateKeysExtractor.Extract(node);

                foreach (var key in intermediateKeys)
                {
                    EnumerateAndAdd(
                        reader,
                        keyToNodeReferenceMapping,
                        keyToNodeReferenceMapping[key],
                        leafKeys);
                }
            }
            else
            {
                leafKeys.AddRange(
                    leafKeysExtractor.Extract(node));
            }
        }
    }
}
