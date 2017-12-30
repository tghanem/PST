using System;

namespace pst.utilities
{
    static class Utilities
    {
        public static int GetTotalInternalDataBlockSize(int rawDataSize)
        {
            return rawDataSize + GetInternalDataBlockPaddingSize(rawDataSize) + 24;
        }

        public static int GetTotalExternalDataBlockSize(int rawDataSize)
        {
            return rawDataSize + GetExternalDataBlockPaddingSize(rawDataSize) + 16;
        }

        public static int GetInternalDataBlockPaddingSize(int rawDataSize)
        {
            return GetRemainingToNextMultipleOf(rawDataSize + 24, 64);
        }

        public static int GetExternalDataBlockPaddingSize(int rawDataSize)
        {
            return GetRemainingToNextMultipleOf(rawDataSize + 16, 64);
        }

        public static int GetRemainingToNextMultipleOf(int number, int multipleOf)
        {
            if (number % multipleOf == 0)
                return 0;

            if (number < multipleOf)
                return multipleOf - number;

            var nextMultipleOf =
                (int)
                Math.Ceiling((double)number / multipleOf) * multipleOf;

            return nextMultipleOf - number;
        }
    }
}