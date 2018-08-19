using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ndb.datatree
{
    class ExternalDataBlockIdsReader : IExternalDataBlockIdsReader
    {
        private readonly IDataReader dataReader;
        private readonly IHeaderReader headerReader;
        private readonly IExtractor<InternalDataBlock, BID[]> blockIdsFromInternalDataBlockExtractor;
        private readonly IBTreeNodeLoader<InternalDataBlock, LBBTEntry> internalDataBlockLoader;
        private readonly IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder;

        public ExternalDataBlockIdsReader(
            IDataReader dataReader,
            IHeaderReader headerReader,
            IExtractor<InternalDataBlock, BID[]> blockIdsFromInternalDataBlockExtractor,
            IBTreeNodeLoader<InternalDataBlock, LBBTEntry> internalDataBlockLoader,
            IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder)
        {
            this.dataReader = dataReader;
            this.headerReader = headerReader;
            this.blockIdsFromInternalDataBlockExtractor = blockIdsFromInternalDataBlockExtractor;
            this.internalDataBlockLoader = internalDataBlockLoader;
            this.blockBTreeEntryFinder = blockBTreeEntryFinder;
        }

        public BID[] Read(LBBTEntry dataTreeEntry)
        {
            var dataTreeRootBlockLevel = GetBlockLevel(dataTreeEntry);

            if (dataTreeRootBlockLevel.HasNoValue)
            {
                return new[] { dataTreeEntry.BlockReference.BlockId };
            }

            return EnumerateAndAdd(dataTreeEntry.BlockReference.BlockId, dataTreeRootBlockLevel.Value);
        }

        private BID[] EnumerateAndAdd(BID blockId, int currentDepth)
        {
            if (currentDepth == 2)
            {
                var blockEntry = GetDataBlockEntry(blockId).Value;

                var internalDataBlock = internalDataBlockLoader.LoadNode(blockEntry);

                var intermediateKeys = blockIdsFromInternalDataBlockExtractor.Extract(internalDataBlock);

                var allLeafKeys = new List<BID>();

                foreach (var key in intermediateKeys)
                {
                    var leafKeys = EnumerateAndAdd(key, currentDepth - 1);

                    allLeafKeys.AddRange(leafKeys);
                }

                return allLeafKeys.ToArray();
            }

            if (currentDepth == 1)
            {
                var blockEntry = GetDataBlockEntry(blockId).Value;

                var internalDataBlock = internalDataBlockLoader.LoadNode(blockEntry);

                return blockIdsFromInternalDataBlockExtractor.Extract(internalDataBlock);
            }

            return new[] { blockId };
        }

        private Maybe<int> GetBlockLevel(LBBTEntry blockEntry)
        {
            var dataBlock = dataReader.Read(blockEntry.BlockReference.ByteIndex.Value, blockEntry.GetBlockSize());

            if (dataBlock.Value[0] == 0x01)
            {
                return dataBlock.Value[1];
            }

            return Maybe<int>.NoValue();
        }

        private Maybe<LBBTEntry> GetDataBlockEntry(BID blockId)
        {
            var header = headerReader.GetHeader();

            return blockBTreeEntryFinder.Find(blockId, header.Root.BBTRootPage);
        }
    }
}
