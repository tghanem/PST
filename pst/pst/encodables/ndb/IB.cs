namespace pst.encodables.ndb
{
    /// <summary>
    /// The IB (Byte Index) is used to represent an absolute offset within the PST file with respect to the beginning of the file.
    /// </summary>
    class IB
    {
        public long Value { get; }

        private IB(long value)
        {
            Value = value;
        }

        public static IB OfValue(long value)
            => new IB(value);
    }
}
