namespace pst.core
{
    public struct Maybe<T> where T : class
    {
        public T Value {get; }

        private Maybe(T value)
        {
            Value = value;
        }

        public bool HasValue => Value != null;

        public bool HasNoValue => Value == null;

        public static Maybe<T> OfValue(T value)
            => new Maybe<T>(value);

        public static Maybe<T> NoValue()
            => new Maybe<T>(null);

        public static implicit operator Maybe<T>(T value)
            => new Maybe<T>(value);
    }
}
