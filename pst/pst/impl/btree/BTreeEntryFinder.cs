using pst.core;
using pst.encodables;
using pst.interfaces;
using System;

namespace pst.impl
{
    class BTreeEntryFinder<TKey, TIntermediateEntry, TLeafEntry> : IBTreeEntryFinder<TKey, TLeafEntry>
        where TIntermediateEntry : class
        where TLeafEntry : class
    {
        private readonly IBTreePageEntryLocator<TKey, TIntermediateEntry> intermediateEntryLocator;
        private readonly IBTreePageEntryLocator<TKey, TLeafEntry> leafEntryLocator;
        private readonly IDataBlockLoader<BTPage> pageDataBlockLoader;
        private readonly BREF rootPageBlockReference;
        private readonly Func<TIntermediateEntry, BREF> intermediateEntryToPageBlockReference;

        public BTreeEntryFinder(
            IBTreePageEntryLocator<TKey, TIntermediateEntry> intermediateEntryLocator,
            IBTreePageEntryLocator<TKey, TLeafEntry> leafEntryLocator,
            IDataBlockLoader<BTPage> pageDataBlockLoader,
            BREF rootPageBlockReference,
            Func<TIntermediateEntry, BREF> intermediateEntryToPageBlockReference)
        {
            this.pageDataBlockLoader = pageDataBlockLoader;
            this.rootPageBlockReference = rootPageBlockReference;
            this.intermediateEntryToPageBlockReference = intermediateEntryToPageBlockReference;
            this.intermediateEntryLocator = intermediateEntryLocator;
            this.leafEntryLocator = leafEntryLocator;
        }

        public Maybe<TLeafEntry> Find(TKey key)
        {
            return Find(key, rootPageBlockReference);
        }

        private Maybe<TLeafEntry> Find(TKey key, BREF pageDataBlockReference)
        {
            var page = pageDataBlockLoader.Load(pageDataBlockReference);

            if (page.PageLevel > 0)
            {
                var entry = intermediateEntryLocator.FindEntry(page, key);

                if (entry.HasNoValue)
                {
                    return Maybe<TLeafEntry>.NoValue<TLeafEntry>();
                }

                return Find(key, intermediateEntryToPageBlockReference(entry.Value));
            }
            else
            {
                return leafEntryLocator.FindEntry(page, key);
            }
        }
    }
}
