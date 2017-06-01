using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ndb.btree
{
    class BTPageDecoder : IDecoder<BTPage>
    {
        private readonly IDecoder<PageTrailer> pageTrailerDecoder;

        public BTPageDecoder(IDecoder<PageTrailer> pageTrailerDecoder)
        {
            this.pageTrailerDecoder = pageTrailerDecoder;
        }

        public BTPage Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new BTPage(
                    parser.TakeAndSkip(488),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(4),
                    parser.TakeAndSkip(16, pageTrailerDecoder));
        }
    }
}
