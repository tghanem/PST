using pst.core;
using pst.interfaces.btree;
using System;
using System.Linq;

namespace pst.impl.btree
{
    class ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<TKey, TReferenceKey> : IBTreeNodeKeysComparer<TKey, TReferenceKey>
        where TReferenceKey : class, IEquatable<TReferenceKey>
        where TKey : class, IEquatable<TKey>
    {
        private readonly Func<TKey, TReferenceKey> keyToReferenceKey;

        public ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey(Func<TKey, TReferenceKey> keyToReferenceKey)
        {
            this.keyToReferenceKey = keyToReferenceKey;
        }

        public Maybe<TKey> GetMatchingKey(TKey[] keys, TReferenceKey referenceKey)
        {
            return
                keys
                .FirstOrDefault(e => keyToReferenceKey(e).Equals(referenceKey));
        }
    }
}
