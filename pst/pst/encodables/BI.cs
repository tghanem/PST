namespace pst.encodables
{
    class BI
    {
        public long Value { get; }

        private BI(long value)
        {
            Value = value;
        }

        public static BI OfValue(long value)
            => new BI(value);
    }
}
