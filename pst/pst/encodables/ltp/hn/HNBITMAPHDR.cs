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

        public static HNBITMAPHDR OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new HNBITMAPHDR(
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(64));
        }
    }
}
