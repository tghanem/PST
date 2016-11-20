namespace pst.core
{
    struct Maybe<T> where T : class
    {
        public T Value {get; }

        private Maybe(T value)
        {
            Value = value;
        }

        public bool HasValue => Value != null;

        public bool HasNoValue => Value == null;

        public static Maybe<TValue> OfValue<TValue>(TValue value) where TValue : class
            => new Maybe<TValue>(value);

        public static Maybe<TValue> NoValue<TValue>() where TValue : class
            => new Maybe<TValue>(null);

        public static implicit operator Maybe<T>(T value)
            => new Maybe<T>(value);
    }
}
