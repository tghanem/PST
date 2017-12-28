using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb.allocation
{
    class AllocationInfo
    {
        public AllocationInfo(IB aMapOffset, int bitStartIndex, int bitEndIndex)
        {
            AMapOffset = aMapOffset;
            BitStartIndex = bitStartIndex;
            BitEndIndex = bitEndIndex;
        }

        public IB AMapOffset { get; }

        public int BitStartIndex { get; }

        public int BitEndIndex { get; }
    }

    interface IAllocationFinder
    {
        Maybe<AllocationInfo> Find(int sizeOfDataInBytes);

        Maybe<AllocationInfo> Find(IB amapOffset, int sizeOfDataInBytes);
    }
}