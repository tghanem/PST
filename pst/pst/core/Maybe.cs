using System;

namespace pst.core
{
    public struct Maybe<T> where T : class
    {
        public T Value
        {
            get
            {
                if (HasNoValue)
                {
                    throw new Exception("BUG: The object does not contain a value");
                }

                return value;
            }
        }

        private readonly T value;

        private Maybe(T value)
        {
            this.value = value;
        }

        public bool HasValue => value != null;

        public bool HasNoValue => value == null;

        public bool HasValueAnd(Func<T, bool> predicate) => HasValue && predicate(Value);

        public static Maybe<T> OfValue(T value)
            => new Maybe<T>(value);

        public static Maybe<T> NoValue()
            => new Maybe<T>(null);

        public static implicit operator Maybe<T>(T value)
            => new Maybe<T>(value);
    }
}
