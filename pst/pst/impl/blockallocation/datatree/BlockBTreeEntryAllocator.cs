using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.blockallocation.datatree
{
    class BlockBTreeEntryAllocator : IBlockBTreeEntryAllocator
    {
        private readonly IHeaderUsageProvider headerUsageProvider;
        private readonly List<LBBTEntry> allocatedBlockBTreeEntries;

        public BlockBTreeEntryAllocator(
            IHeaderUsageProvider headerUsageProvider,
            List<LBBTEntry> allocatedBlockBTreeEntries)
        {
            this.headerUsageProvider = headerUsageProvider;
            this.allocatedBlockBTreeEntries = allocatedBlockBTreeEntries;
        }

        public BID Allocate(IB blockOffset, int rawDataSize, bool internalBlock)
        {
            var bidIndex = headerUsageProvider.GetHeader().NextBID;

            headerUsageProvider.Use(header => header.IncrementBIDIndexForDataBlocks());

            var blockId = internalBlock ? BID.ForInternalBlock(bidIndex) : BID.ForExternalBlock(bidIndex);

            allocatedBlockBTreeEntries.Add(
                new LBBTEntry(
                    BREF.OfValue(blockId, blockOffset),
                    rawDataSize,
                    numberOfReferencesToThisBlock: 1,
                    padding: BinaryData.OfSize(4)));

            return blockId;
        }
    }
}
