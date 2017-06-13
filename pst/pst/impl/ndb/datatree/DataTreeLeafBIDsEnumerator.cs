using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using System.Collections.Generic;

namespace pst.impl.ndb.datatree
{
    class DataTreeLeafBIDsEnumerator : IDataTreeLeafBIDsEnumerator
    {
        private readonly IDataTreeBlockLevelDecider dataTreeBlockLevelDecider;
        private readonly IBTreeNodeLoader<InternalDataBlock, BID> internalDataBlockLoader;
        private readonly IExtractor<InternalDataBlock, BID[]> blockIdsFromInternalDataBlockExtractor;

        public DataTreeLeafBIDsEnumerator(
            IDataTreeBlockLevelDecider dataTreeBlockLevelDecider,
            IBTreeNodeLoader<InternalDataBlock, BID> internalDataBlockLoader,
            IExtractor<InternalDataBlock, BID[]> blockIdsFromInternalDataBlockExtractor)
        {
            this.dataTreeBlockLevelDecider = dataTreeBlockLevelDecider;
            this.internalDataBlockLoader = internalDataBlockLoader;
            this.blockIdsFromInternalDataBlockExtractor = blockIdsFromInternalDataBlockExtractor;
        }

        public BID[] Enumerate(BID blockId)
        {
            return
                EnumerateAndAdd(
                    blockId,
                    dataTreeBlockLevelDecider.GetBlockLevel(blockId));
        }

        private BID[] EnumerateAndAdd(BID blockId, int currentDepth)
        {
            if (currentDepth == 2)
            {
                var internalDataBlock = internalDataBlockLoader.LoadNode(blockId);

                var intermediateKeys = blockIdsFromInternalDataBlockExtractor.Extract(internalDataBlock);

                var allLeafKeys = new List<BID>();

                foreach (var key in intermediateKeys)
                {
                    var leafKeys = EnumerateAndAdd(key, currentDepth - 1);

                    allLeafKeys.AddRange(leafKeys);
                }

                return allLeafKeys.ToArray();
            }
            else if (currentDepth == 1)
            {
                var internalDataBlock = internalDataBlockLoader.LoadNode(blockId);

                return blockIdsFromInternalDataBlockExtractor.Extract(internalDataBlock);
            }

            return new[] { blockId };
        }
    }
}
