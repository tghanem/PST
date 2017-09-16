using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using System.Collections.Generic;

namespace pst.impl.ndb
{
    class DataBlockEntryFinder : IDataBlockEntryFinder
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;
        private readonly IExtractor<InternalDataBlock, BID[]> blockIdsFromInternalDataBlockExtractor;
        private readonly IBTreeNodeLoader<InternalDataBlock, LBBTEntry> internalDataBlockLoader;
        private readonly IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder;

        public DataBlockEntryFinder(
            IDataReader dataReader,
            IDecoder<Header> headerDecoder,
            IExtractor<InternalDataBlock, BID[]> blockIdsFromInternalDataBlockExtractor,
            IBTreeNodeLoader<InternalDataBlock, LBBTEntry> internalDataBlockLoader,
            IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
            this.blockIdsFromInternalDataBlockExtractor = blockIdsFromInternalDataBlockExtractor;
            this.internalDataBlockLoader = internalDataBlockLoader;
            this.blockBTreeEntryFinder = blockBTreeEntryFinder;
        }

        public Maybe<DataBlockEntry> Find(BID blockId)
        {
            var lbbtEntry = GetDataBlockEntry(blockId);

            if (lbbtEntry.HasNoValue)
            {
                return Maybe<DataBlockEntry>.NoValue();
            }

            var childBlockIds = EnumerateAndAdd(blockId, GetBlockLevel(lbbtEntry.Value));

            return Maybe<DataBlockEntry>.OfValue(new DataBlockEntry(lbbtEntry.Value, childBlockIds));
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

        private int GetBlockLevel(LBBTEntry blockEntry)
        {
            var dataBlock = dataReader.Read(blockEntry.BlockReference.ByteIndex.Value, blockEntry.GetBlockSize());

            if (dataBlock.Value[0] == 0x01)
            {
                return dataBlock.Value[1];
            }

            return 0;
        }

        private Maybe<LBBTEntry> GetDataBlockEntry(BID blockId)
        {
            var header = headerDecoder.Decode(dataReader.Read(0, 546));

            return blockBTreeEntryFinder.Find(blockId, header.Root.BBTRootPage);
        }
    }
}
