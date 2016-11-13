using System;

namespace pst.utilities
{
    static class ExtensionMethods
    {
        public static void AssertNotNullAndLength(this BinaryData @data, int expectedLength)
        {
            if (@data == null)
                throw new ArgumentNullException();

            if (@data.Length != expectedLength)
                throw new ArgumentException("Data length is not expected");
        }
    }
}
