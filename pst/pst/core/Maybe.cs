using System;

namespace pst.core
{
    public struct Maybe<T>
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
        private readonly bool hasValue;

        private Maybe(T value, bool hasValue)
        {
            this.value = value;
            this.hasValue = hasValue;
        }

        public bool HasValue => hasValue;

        public bool HasNoValue => !hasValue;

        public bool HasValueAnd(Func<T, bool> predicate) => HasValue && predicate(Value);

        public static Maybe<T> OfValue(T value)
        {
            if (value == null)
            {
                throw new Exception("BUG: Cannot instantiate a Maybe<T> of null value");
            }

            return new Maybe<T>(value, hasValue: true);
        }

        public static Maybe<T> NoValue()
        {
            return new Maybe<T>(default(T), hasValue: false);
        }

        public static implicit operator Maybe<T>(T value)
        {
            return new Maybe<T>(value, hasValue: value != null);
        }
    }
}
