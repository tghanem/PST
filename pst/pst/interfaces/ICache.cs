using pst.core;
using System;

namespace pst.interfaces
{
    interface ICache<TKey, TValue> where TValue : class
    {
        void Add(TKey key, TValue value);

        bool HasValue(TKey key);

        TValue GetValue(TKey key);

        Maybe<TValue> GetOrAdd(TKey key, Func<Maybe<TValue>> getValue);
    }
}