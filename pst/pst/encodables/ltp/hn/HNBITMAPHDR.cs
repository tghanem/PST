using pst.utilities;

namespace pst.encodables.ltp.hn
{
    class HNBITMAPHDR
    {
        ///2
        public int PageMapOffset { get; }

        ///64
        public BinaryData FillLevel { get; }

        public HNBITMAPHDR(int pageMapOffset, BinaryData fillLevel)
        {
            PageMapOffset = pageMapOffset;
            FillLevel = fillLevel;
        }
    }
}
