using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    class AllocationInfo
    {
        public AllocationInfo(IB aMapOffset, int bitIndex)
        {
            AMapOffset = aMapOffset;
            BitIndex = bitIndex;
        }

        public IB AMapOffset { get; }

        public int BitIndex { get; }
    }

    interface IAllocationFinder
    {
        Maybe<AllocationInfo> Find(int sizeInBytes);
    }
}