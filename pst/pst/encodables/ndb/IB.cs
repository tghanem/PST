namespace pst.encodables.ndb
{
    class IB
    {
        public long Value { get; }

        private IB(long value)
        {
            Value = value;
        }

        public static IB Zero = OfValue(0);

        public static IB OfValue(long value) => new IB(value);

        public override string ToString()
        {
            return $"0x{Value.ToString("X")}";
        }
    }
}
