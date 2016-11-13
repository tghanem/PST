namespace pst.encodables
{
    class BID
    {
        public long Value { get; }

        private BID(long value)
        {
            Value = value;
        }

        public static BID OfValue(long value)
            => new BID(value);
    }
}
