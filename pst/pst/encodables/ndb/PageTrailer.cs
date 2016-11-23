namespace pst.encodables
{
    class PageTrailer
    {
        public int PageType { get; }

        public int PageTypeRepeat { get; }

        public int PageSignature { get; }

        public int Crc32ForPageData { get; }

        public BID PageBlockId { get; }

        public PageTrailer(int pageType, int pageTypeRepeat, int pageSignature, int crc32ForPageData, BID pageBlockId)
        {
            PageType = pageType;
            PageTypeRepeat = pageTypeRepeat;
            PageSignature = pageSignature;
            Crc32ForPageData = crc32ForPageData;
            PageBlockId = pageBlockId;
        }
    }
}
