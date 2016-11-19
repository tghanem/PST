using pst.utilities;

namespace pst.encodables
{
    class LBBTPage
    {
        public LBBTEntry[] Entries { get; }

        public int NumberOfEntriesInPage { get; }

        public int MaximumNumberOfEntriesInPage { get; }

        public int EntrySize { get; }

        public int PageLevel { get; }

        public BinaryData Padding { get; }

        public PageTrailer PageTrailer { get; }

        public LBBTPage(LBBTEntry[] entries, int numberOfEntriesInPage, int maximumNumberOfEntriesInPage, int entrySize, int pageLevel, BinaryData padding, PageTrailer pageTrailer)
        {
            Entries = entries;
            NumberOfEntriesInPage = numberOfEntriesInPage;
            MaximumNumberOfEntriesInPage = maximumNumberOfEntriesInPage;
            EntrySize = entrySize;
            PageLevel = pageLevel;
            Padding = padding;
            PageTrailer = pageTrailer;
        }
    }
}
