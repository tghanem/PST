using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.decoders.ndb
{
    class PageTrailerDecoder : IDecoder<PageTrailer>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<BID> bidDecoder;

        public PageTrailerDecoder(IDecoder<int> int32Decoder, IDecoder<BID> bidDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.bidDecoder = bidDecoder;
        }

        public PageTrailer Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new PageTrailer(
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(4, int32Decoder),
                        parser.TakeAndSkip(8, bidDecoder));
            }
        }
    }
}
