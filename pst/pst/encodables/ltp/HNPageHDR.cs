namespace pst.encodables.ltp
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
