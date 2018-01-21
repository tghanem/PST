using pst.core;

namespace pst.interfaces
{
    interface IReadOnlyDataHolder<T>
    {
        Maybe<T> GetData();
    }

    interface IWriteOnlyDataHolder<T>
    {
        void SetData(T value);
    }

    interface IDataHolder<T> : IReadOnlyDataHolder<T>, IWriteOnlyDataHolder<T>
    {
    }
}