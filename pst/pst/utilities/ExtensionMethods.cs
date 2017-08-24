using System;
using System.Collections.Generic;
using pst.interfaces;

namespace pst.utilities
{
    static class ExtensionMethods
    {
        public static T[] DecodeMultipleItems<T>(this IDecoder<T> decoder, int numberOfItems, int itemSize, BinaryData data)
        {
            var entries = new List<T>();

            for (var i = 0; i < numberOfItems; i++)
            {
                var item = data.Take(i * itemSize, itemSize);

                entries.Add(decoder.Decode(item));
            }

            return entries.ToArray();
        }

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
