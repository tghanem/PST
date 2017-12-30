using System.Collections.Generic;
using pst.encodables.ndb;
using pst.interfaces.blockallocation.datatree;
using pst.utilities;
using System.Linq;

namespace pst.impl.blockallocation.datatree
{
    class DataTreeAllocator : IDataTreeAllocator
    {
        private const int InternalDataBlockCapacityExcludingTheBlockTrailer = 8 * 1024 - 24;
        private const int MaximumNumberOfBIDEntriesInInternalBlock = InternalDataBlockCapacityExcludingTheBlockTrailer / 8;

        private readonly IDataBlockAllocator<BinaryData> externalDataBlockAllocator;
        private readonly IDataBlockAllocator<BlockIdsWithTotalNumberOfBytesInReferencedBlocks> internalXBlockAllocator;
        private readonly IDataBlockAllocator<BlockIdsWithTotalNumberOfBytesInReferencedBlocks> internalXXBlockAllocator;

        public DataTreeAllocator(
            IDataBlockAllocator<BinaryData> externalDataBlockAllocator,
            IDataBlockAllocator<BlockIdsWithTotalNumberOfBytesInReferencedBlocks> internalXBlockAllocator,
            IDataBlockAllocator<BlockIdsWithTotalNumberOfBytesInReferencedBlocks> internalXXBlockAllocator)
        {
            this.externalDataBlockAllocator = externalDataBlockAllocator;
            this.internalXBlockAllocator = internalXBlockAllocator;
            this.internalXXBlockAllocator = internalXXBlockAllocator;
        }

        public BREF Allocate(BinaryData[] dataPerExternalBlock)
        {
            if (dataPerExternalBlock.Length == 1)
            {
                return externalDataBlockAllocator.Allocate(dataPerExternalBlock[0]);
            }

            if (dataPerExternalBlock.Length > 1 && dataPerExternalBlock.Length < MaximumNumberOfBIDEntriesInInternalBlock)
            {
                return AllocateXBlock(dataPerExternalBlock);
            }

            return AllocateXXBlock(dataPerExternalBlock);
        }

        private BREF AllocateXXBlock(BinaryData[] dataPerExternalBlock)
        {
            var slicesOfDataPerExternalBlock = dataPerExternalBlock.Slice(MaximumNumberOfBIDEntriesInInternalBlock);

            var blockReferences = new List<BID>();

            foreach (var slice in slicesOfDataPerExternalBlock)
            {
                var allocateXBlockReference = AllocateXBlock(slice);

                blockReferences.Add(allocateXBlockReference.BlockId);
            }

            var totalBytesInReferencedExternalBlocks = dataPerExternalBlock.Aggregate(0, (sum, data) => sum + data.Length);

            return
                internalXXBlockAllocator.Allocate(
                    new BlockIdsWithTotalNumberOfBytesInReferencedBlocks(blockReferences.ToArray(),
                        totalBytesInReferencedExternalBlocks));
        }

        private BREF AllocateXBlock(BinaryData[] dataPerExternalBlock)
        {
            var externalBlockIds =
                dataPerExternalBlock
                .Select(d => externalDataBlockAllocator.Allocate(d))
                .Select(r => r.BlockId)
                .ToArray();

            var totalBytesInReferencedExternalBlocks = dataPerExternalBlock.Aggregate(0, (sum, data) => sum + data.Length);

            return
                internalXBlockAllocator.Allocate(
                    new BlockIdsWithTotalNumberOfBytesInReferencedBlocks(
                        externalBlockIds,
                        totalBytesInReferencedExternalBlocks));
        }
    }
}
