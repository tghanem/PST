using pst.core;
using pst.interfaces;
using System;
using System.Collections.Generic;

namespace pst.impl
{
    class DictionaryBasedCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly Dictionary<TKey, TValue> cache;

        public DictionaryBasedCache()
        {
            cache = new Dictionary<TKey, TValue>();
        }

        public Maybe<TValue> GetOrAdd(TKey key, Func<Maybe<TValue>> getValue)
        {
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var value = getValue();

            if (value.HasValue)
            {
                cache.Add(key, value.Value);
            }

            return value;
        }

        public void Add(TKey key, TValue value)
        {
            cache.Add(key, value);
        }

        public bool HasValue(TKey key)
        {
            return cache.ContainsKey(key);
        }

        public TValue GetValue(TKey key)
        {
            return cache[key];
        }
    }
}
