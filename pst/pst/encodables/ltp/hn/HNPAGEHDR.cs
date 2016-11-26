namespace pst.encodables.ltp.hn
{
    class HNPAGEHDR
    {
        ///2
        public int PageMapOffset { get; }

        public HNPAGEHDR(int pageMapOffset)
        {
            PageMapOffset = pageMapOffset;
        }
    }
}
