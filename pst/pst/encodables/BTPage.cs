using pst.utilities;

namespace pst.interfaces
{
    class BTPage
    {
        public BinaryData RGEntries { get; }

        public int NumberOfEntriesInPage { get; }

        public int MaximumNumberOfEntriesInPage { get; }

        public int EntrySize { get; }

        public int PageLevel { get; }

        public BinaryData Padding { get; }

        public BinaryData PageTrailer { get; }

        public BTPage(
            BinaryData rgEntries,
            int numberOfEntriesInPage,
            int maximumNumberOfEntriesInPage,
            int entrySize,
            int pageLevel,
            BinaryData padding,
            BinaryData pageTrailer)
        {
            RGEntries = rgEntries;
            NumberOfEntriesInPage = numberOfEntriesInPage;
            MaximumNumberOfEntriesInPage = maximumNumberOfEntriesInPage;
            EntrySize = entrySize;
            PageLevel = pageLevel;
            Padding = padding;
            PageTrailer = pageTrailer;
        }
    }
}
