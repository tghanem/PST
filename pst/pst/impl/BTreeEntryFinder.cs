using pst.encodables;
using pst.interfaces;
using System;

namespace pst.impl
{
    class BTreeEntryFinder<TKey, TIntermediateEntry, TLeafEntry> : IBTreeEntryFinder<TKey, TIntermediateEntry, TLeafEntry>
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

        public TLeafEntry Find(TKey key)
        {
            return Find(key, rootPageBlockReference);
        }

        private TLeafEntry Find(TKey key, BREF pageBlockReference)
        {
            var page =
                pageLoader.LoadPage(pageBlockReference);

            if (page.PageLevel > 0)
            {
                var entry =
                    intermediateEntryLocator.FindEntry(page, key);

                return
                    Find(
                        key,
                        intermediateEntryToPageBlockReference(entry));
            }
            else
            {
                return
                    leafEntryLocator.FindEntry(page, key);
            }
        }
    }
}
