using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using System.Collections.Generic;

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

        private readonly IMapper<TIntermediateKey, TNodeReference> keyToNodeReferenceMapping;
        private readonly IDataBlockReader<TNodeReference> dataBlockReader;

        public BTreeLeafKeyEnumeratorThatDoesntKnowHowToMapKeyToNodeReference(
            IExtractor<TNode, TIntermediateKey[]> intermediateKeysExtractor,
            IExtractor<TNode, TLeafKey[]> leafKeysExtractor,
            IExtractor<TNode, int> nodeLevelFromNodeExtractor,
            IBTreeNodeLoader<TNode, TNodeReference> nodeLoader,
            IMapper<TIntermediateKey, TNodeReference> keyToNodeReferenceMapping,
            IDataBlockReader<TNodeReference> dataBlockReader)
        {
            this.intermediateKeysExtractor = intermediateKeysExtractor;
            this.leafKeysExtractor = leafKeysExtractor;
            this.nodeLevelFromNodeExtractor = nodeLevelFromNodeExtractor;
            this.nodeLoader = nodeLoader;
            this.keyToNodeReferenceMapping = keyToNodeReferenceMapping;
            this.dataBlockReader = dataBlockReader;
        }

        public TLeafKey[] Enumerate(TNodeReference rootNodeReference)
        {
            var leafKeys = new List<TLeafKey>();

            EnumerateAndAdd(rootNodeReference, leafKeys);

            return leafKeys.ToArray();
        }

        private void EnumerateAndAdd(TNodeReference nodeReference, List<TLeafKey> leafKeys)
        {
            var node = nodeLoader.LoadNode(nodeReference);

            if (nodeLevelFromNodeExtractor.Extract(node) > 0)
            {
                var intermediateKeys = intermediateKeysExtractor.Extract(node);

                foreach (var key in intermediateKeys)
                {
                    EnumerateAndAdd(keyToNodeReferenceMapping.Map(key), leafKeys);
                }
            }
            else
            {
                leafKeys.AddRange(leafKeysExtractor.Extract(node));
            }
        }
    }
}
