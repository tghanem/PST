namespace pst.encodables.ndb
{
    class IB
    {
        public static readonly IB Zero = OfValue(0);

        public long Value { get; }

        private IB(long value)
        {
            Value = value;
        }

        public static IB OfValue(long value) => new IB(value);

        public IB Add(long value)
        {
            return new IB(Value + value);
        }

        public IB Subtract(long value)
        {
            return new IB(Value - value);
        }

        public static implicit operator long(IB offset)
        {
            return offset.Value;
        }

        public static long operator +(IB offset, long value)
        {
            return offset.Value + value;
        }

        public static double operator /(IB offset, double value)
        {
            return offset.Value / value;
        }

        public static double operator %(IB offset, double value)
        {
            return offset.Value % value;
        }

        public override string ToString()
        {
            return $"0x{Value:x}".ToLower();
        }
    }
}
