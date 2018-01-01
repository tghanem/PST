using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.interfaces.io;
using pst.interfaces.rawallocation;
using pst.utilities;

namespace pst.impl.blockallocation.datatree
{
    class DataBlockAllocator<TType, TValue> : IDataBlockAllocator<TValue>
    {
        private readonly IRawDataAllocator rawDataAllocator;
        private readonly IBlockBTreeEntryAllocator blockBTreeEntryAllocator;
        private readonly IRegionInitializer<TType> blockRegionInitializer;
        private readonly IDataBlockFactory<TValue, TType> blockFactory;
        private readonly IExtractor<TValue, int> valueSizeExtractor;
        private readonly bool isInternal;

        public DataBlockAllocator(
            IRawDataAllocator rawDataAllocator,
            IBlockBTreeEntryAllocator blockBTreeEntryAllocator,
            IRegionInitializer<TType> blockRegionInitializer,
            IDataBlockFactory<TValue, TType> blockFactory,
            IExtractor<TValue, int> valueSizeExtractor,
            bool isInternal)
        {
            this.rawDataAllocator = rawDataAllocator;
            this.blockBTreeEntryAllocator = blockBTreeEntryAllocator;
            this.blockRegionInitializer = blockRegionInitializer;
            this.blockFactory = blockFactory;
            this.valueSizeExtractor = valueSizeExtractor;
            this.isInternal = isInternal;
        }

        public BID Allocate(TValue rawValue)
        {
            var valueSize = valueSizeExtractor.Extract(rawValue);

            var totalDataBlockSize =
                isInternal
                ? Utilities.GetTotalInternalDataBlockSize(valueSize)
                : Utilities.GetTotalExternalDataBlockSize(valueSize);

            var blockOffset = rawDataAllocator.Allocate(totalDataBlockSize);

            var blockId = blockBTreeEntryAllocator.Allocate(blockOffset, valueSize, isInternal);

            var type = blockFactory.Create(blockOffset, blockId, rawValue);

            blockRegionInitializer.Initialize(blockOffset, type);

            return blockId;
        }
    }
}
