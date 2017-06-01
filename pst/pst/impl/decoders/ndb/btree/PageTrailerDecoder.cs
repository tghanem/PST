using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.decoders.ndb
{
    class PageTrailerDecoder : IDecoder<PageTrailer>
    {
        private readonly IDecoder<BID> bidDecoder;

        public PageTrailerDecoder(IDecoder<BID> bidDecoder)
        {
            this.bidDecoder = bidDecoder;
        }

        public PageTrailer Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new PageTrailer(
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(8, bidDecoder));
        }
    }
}
