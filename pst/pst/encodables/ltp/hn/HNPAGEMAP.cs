using pst.utilities;

namespace pst.encodables.ltp.hn
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

        public static HNPAGEMAP OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var allocationCount = parser.TakeAndSkip(2).ToInt32();
            var freeCount = parser.TakeAndSkip(2).ToInt32();
            var allocationTable = parser.TakeAndSkip((allocationCount + 1) * 2);

            return new HNPAGEMAP(allocationCount, freeCount, allocationTable);
        }
    }
}
