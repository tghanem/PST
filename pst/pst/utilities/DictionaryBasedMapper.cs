using pst.interfaces;
using System.Collections.Generic;

namespace pst.utilities
{
    class DictionaryBasedMapper<TKey, TValue> : IMapper<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;

        public DictionaryBasedMapper(Dictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public TValue Map(TKey input)
        {
            return dictionary[input];
        }
    }
}
