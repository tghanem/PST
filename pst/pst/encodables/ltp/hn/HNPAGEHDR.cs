using pst.utilities;

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

        public static HNPAGEHDR OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return new HNPAGEHDR(parser.TakeAndSkip(2).ToInt32());
        }
    }
}
