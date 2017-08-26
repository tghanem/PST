using System;
using pst.core;

namespace pst.interfaces
{
    interface ICache<TKey, TValue> where TValue : class
    {
        Maybe<TValue> GetOrAdd(TKey key, Func<Maybe<TValue>> getValue);
    }
}