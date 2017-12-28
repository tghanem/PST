using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb.allocation
{
    class AllocationInfo
    {
        public AllocationInfo(IB mapOffset, int bitStartIndex, int bitEndIndex)
        {
            MapOffset = mapOffset;
            BitStartIndex = bitStartIndex;
            BitEndIndex = bitEndIndex;
        }

        public IB MapOffset { get; }

        public int BitStartIndex { get; }

        public int BitEndIndex { get; }
    }

    interface IAllocationFinder
    {
        Maybe<AllocationInfo> Find(int sizeOfDataInBytes);

        Maybe<AllocationInfo> Find(IB mapOffset, int sizeOfDataInBytes);
    }
}