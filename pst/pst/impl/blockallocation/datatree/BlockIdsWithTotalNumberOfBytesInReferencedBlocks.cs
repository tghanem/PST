using pst.encodables.ndb;

namespace pst.impl.blockallocation.datatree
{
    class BlockIdsWithTotalNumberOfBytesInReferencedBlocks
    {
        public BlockIdsWithTotalNumberOfBytesInReferencedBlocks(BID[] blockIds, int totalNumberOfBytesInReferencedBlocks)
        {
            BlockIds = blockIds;
            TotalNumberOfBytesInReferencedBlocks = totalNumberOfBytesInReferencedBlocks;
        }

        public BID[] BlockIds { get; }

        public int TotalNumberOfBytesInReferencedBlocks { get; }
    }
}
