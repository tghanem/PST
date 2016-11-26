using System;

namespace pst.utilities
{
    static class ExtensionMethods
    {
        public static int GetRemainingToNextMultipleOf(this int @number, int multipleOf)
        {
            if (@number % multipleOf == 0)
                return 0;

            if (@number < multipleOf)
                return multipleOf - @number;

            var nextMultipleOf =
                (int)
                Math.Ceiling((double)@number / multipleOf) * multipleOf;

            return nextMultipleOf - @number;
        }
    }
}
