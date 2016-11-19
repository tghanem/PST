using pst.utilities;

namespace pst.encodables
{
    class LNBTPage
    {
        public LNBTEntry[] Entries { get; }

        public int NumberOfEntries { get; }

        public int MaximumNumberOfEntries { get; }

        public int SizeOfEachEntry { get; }

        public int PageLevel { get; }

        public BinaryData Padding { get; }

        public PageTrailer PageTrailer { get; }

        public LNBTPage(LNBTEntry[] entries, int numberOfEntries, int maximumNumberOfEntries, int sizeOfEachEntry, int pageLevel, BinaryData padding, PageTrailer pageTrailer)
        {
            Entries = entries;
            NumberOfEntries = numberOfEntries;
            MaximumNumberOfEntries = maximumNumberOfEntries;
            SizeOfEachEntry = sizeOfEachEntry;
            PageLevel = pageLevel;
            Padding = padding;
            PageTrailer = pageTrailer;
        }
    }
}
