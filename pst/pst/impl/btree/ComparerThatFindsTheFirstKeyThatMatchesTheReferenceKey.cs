using pst.core;
using pst.interfaces;
using pst.interfaces.btree;
using System;
using System.Linq;

namespace pst.impl.btree
{
    class ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<TKey, TReferenceKey> : IBTreeNodeKeysComparer<TKey, TReferenceKey>
        where TReferenceKey : class, IEquatable<TReferenceKey>
        where TKey : class
    {
        private readonly IExtractor<TKey, TReferenceKey> referenceKeyFromKeyExtractor;

        public ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey(IExtractor<TKey, TReferenceKey> referenceKeyFromKeyExtractor)
        {
            this.referenceKeyFromKeyExtractor = referenceKeyFromKeyExtractor;
        }

        public Maybe<TKey> GetMatchingKey(TKey[] keys, TReferenceKey referenceKey)
        {
            return
                keys
                .FirstOrDefault(e => referenceKeyFromKeyExtractor.Extract(e).Equals(referenceKey));
        }
    }
}
