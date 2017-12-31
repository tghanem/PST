namespace pst.encodables.ndb.btree
{
    class PageTrailer
    {
        //1
        public int PageType { get; }

        //1
        public int PageTypeRepeat { get; }

        //2
        public int PageSignature { get; }

        //4
        public int Crc32ForPageData { get; }

        //8
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
