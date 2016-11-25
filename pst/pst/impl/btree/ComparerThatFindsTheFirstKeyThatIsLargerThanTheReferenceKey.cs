using pst.core;
using pst.interfaces.btree;
using System;
using System.Linq;

namespace pst.impl.btree
{
    class ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<TKey, TReferenceKey> : IBTreeNodeKeysComparer<TKey, TReferenceKey>
        where TReferenceKey : class, IComparable<TReferenceKey>
        where TKey : class, IComparable<TKey>
    {
        private readonly Func<TKey, TReferenceKey> keyToReferenceKey;
        
        public ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey(Func<TKey, TReferenceKey> keyToReferenceKey)
        {
            this.keyToReferenceKey = keyToReferenceKey;
        }

        public Maybe<TKey> GetMatchingKey(TKey[] keys, TReferenceKey key)
        {
            return
                keys
                .FirstOrDefault(k => keyToReferenceKey(k).CompareTo(key) <= 0);
        }
    }
}
