using pst.core;
using pst.interfaces;
using pst.interfaces.btree;
using System;
using System.Linq;

namespace pst.impl.btree
{
    class ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<TKey, TReferenceKey> : IBTreeNodeKeysComparer<TKey, TReferenceKey>
        where TReferenceKey : class, IComparable<TReferenceKey>
        where TKey : class
    {
        private readonly IExtractor<TKey, TReferenceKey> referenceKeyFromKeyExtractor;

        public ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey(IExtractor<TKey, TReferenceKey> referenceKeyFromKeyExtractor)
        {
            this.referenceKeyFromKeyExtractor = referenceKeyFromKeyExtractor;
        }

        public Maybe<TKey> GetMatchingKey(TKey[] keys, TReferenceKey key)
        {
            var referenceKeys =
                keys
                .Select(referenceKeyFromKeyExtractor.Extract)
                .ToArray();

            for (var i = 0; i < referenceKeys.Length - 1; i++)
            {
                if (key.CompareTo(referenceKeys[i]) >= 0 &&
                    key.CompareTo(referenceKeys[i + 1]) < 0)
                {
                    return keys[i];
                }
            }

            return keys[keys.Length - 1];
        }
    }
}
