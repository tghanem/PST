using pst.core;
using pst.interfaces;

namespace pst.impl
{
    class DefaultDataHolder<T> : IDataHolder<T>
    {
        private Maybe<T> data;

        public Maybe<T> GetData() => data;

        public void SetData(T value) => data = Maybe<T>.OfValue(value);
    }
}
