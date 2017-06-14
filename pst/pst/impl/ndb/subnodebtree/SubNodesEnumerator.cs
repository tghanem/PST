using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using System.Collections.Generic;

namespace pst.impl.ndb.subnodebtree
{
    class SubNodesEnumerator : ISubNodesEnumerator
    {
        private readonly ISubnodeBTreeBlockLevelDecider subnodeBTreeBlockLevelDecider;
        private readonly IBTreeNodeLoader<SubnodeBlock, BID> subnodeBlockLoader;
        private readonly IExtractor<SubnodeBlock, SIEntry[]> entriesFromIntermediateSubnodeBlockExtractor;
        private readonly IExtractor<SubnodeBlock, SLEntry[]> entriesFromLeafSubnodeBlockExtractor;

        public SubNodesEnumerator(
            ISubnodeBTreeBlockLevelDecider subnodeBTreeBlockLevelDecider,
            IBTreeNodeLoader<SubnodeBlock, BID> subnodeBlockLoader,
            IExtractor<SubnodeBlock, SIEntry[]> entriesFromIntermediateSubnodeBlockExtractor,
            IExtractor<SubnodeBlock, SLEntry[]> entriesFromLeafSubnodeBlockExtractor)
        {
            this.subnodeBTreeBlockLevelDecider = subnodeBTreeBlockLevelDecider;
            this.subnodeBlockLoader = subnodeBlockLoader;
            this.entriesFromIntermediateSubnodeBlockExtractor = entriesFromIntermediateSubnodeBlockExtractor;
            this.entriesFromLeafSubnodeBlockExtractor = entriesFromLeafSubnodeBlockExtractor;
        }

        public SLEntry[] Enumerate(BID subnodeDataBlockId)
        {
            return
                EnumerateAndAdd(
                    subnodeDataBlockId,
                    subnodeBTreeBlockLevelDecider.GetBlockLevel(subnodeDataBlockId));
        }

        private SLEntry[] EnumerateAndAdd(BID blockId, int currentDepth)
        {
            if (currentDepth == 1)
            {
                var subnodeBlock = subnodeBlockLoader.LoadNode(blockId);

                var intermediateKeys = entriesFromIntermediateSubnodeBlockExtractor.Extract(subnodeBlock);

                var allLeafKeys = new List<SLEntry>();

                foreach (var key in intermediateKeys)
                {
                    var leafKeys = EnumerateAndAdd(key.SLBlockId, currentDepth - 1);

                    allLeafKeys.AddRange(leafKeys);
                }

                return allLeafKeys.ToArray();
            }
            else
            {
                var subnodeBlock = subnodeBlockLoader.LoadNode(blockId);

                return entriesFromLeafSubnodeBlockExtractor.Extract(subnodeBlock);
            }
        }
    }
}
