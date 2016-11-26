using pst.utilities;

namespace pst.encodables.ltp
{
    class HNPAGEMAP
    {
        ///2
        public int AllocationCount { get; }

        ///2
        public int FreeCount { get; }

        ///(AllocationCount + 1) * 2
        public BinaryData AllocationTable { get; }

        public HNPAGEMAP(int allocationCount, int freeCount, BinaryData allocationTable)
        {
            AllocationCount = allocationCount;
            FreeCount = freeCount;
            AllocationTable = allocationTable;
        }
    }
}
