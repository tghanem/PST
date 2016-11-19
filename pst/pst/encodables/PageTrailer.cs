namespace pst.encodables
{
    class PageTrailer
    {
        public int Type { get; }

        public int TypeRepeat { get; }

        public int PageSignature { get; }

        public int Crc32ForPageData { get; }

        public BID PageBlockId { get; }

        public PageTrailer(int type, int typeRepeat, int pageSignature, int crc32ForPageData, BID pageBlockId)
        {
            Type = type;
            TypeRepeat = typeRepeat;
            PageSignature = pageSignature;
            Crc32ForPageData = crc32ForPageData;
            PageBlockId = pageBlockId;
        }
    }
}
