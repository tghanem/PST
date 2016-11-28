using pst.core;
using pst.interfaces;
using pst.interfaces.btree;
using System;
using System.Linq;

namespace pst.impl.btree
{
    class ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<TKey, TReferenceKey> : IBTreeNodeKeysComparer<TKey, TReferenceKey>
        where TReferenceKey : class, IComparable<TReferenceKey>
        where TKey : class, IComparable<TKey>
    {
        private readonly IExtractor<TKey, TReferenceKey> referenceKeyFromKeyExtractor;
        
        public ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey(IExtractor<TKey, TReferenceKey> referenceKeyFromKeyExtractor)
        {
            this.referenceKeyFromKeyExtractor = referenceKeyFromKeyExtractor;
        }

        public Maybe<TKey> GetMatchingKey(TKey[] keys, TReferenceKey key)
        {
            return
                keys
                .FirstOrDefault(k => referenceKeyFromKeyExtractor.Extract(k).CompareTo(key) <= 0);
        }
    }
}
