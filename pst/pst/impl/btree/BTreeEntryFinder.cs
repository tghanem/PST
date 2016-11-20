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
        private readonly IBTreePageLoader pageLoader;
        private readonly BREF rootPageBlockReference;
        private readonly Func<TIntermediateEntry, BREF> intermediateEntryToPageBlockReference;

        public BTreeEntryFinder(
            IBTreePageEntryLocator<TKey, TIntermediateEntry> intermediateEntryLocator,
            IBTreePageEntryLocator<TKey, TLeafEntry> leafEntryLocator,
            IBTreePageLoader pageLoader,
            BREF rootPageBlockReference,
            Func<TIntermediateEntry, BREF> intermediateEntryToPageBlockReference)
        {
            this.pageLoader = pageLoader;
            this.rootPageBlockReference = rootPageBlockReference;
            this.intermediateEntryToPageBlockReference = intermediateEntryToPageBlockReference;
            this.intermediateEntryLocator = intermediateEntryLocator;
            this.leafEntryLocator = leafEntryLocator;
        }

        public Maybe<TLeafEntry> Find(TKey key)
        {
            return Find(key, rootPageBlockReference);
        }

        private Maybe<TLeafEntry> Find(TKey key, BREF pageBlockReference)
        {
            var page = pageLoader.LoadPage(pageBlockReference);

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
