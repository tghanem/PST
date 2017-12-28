using pst.encodables.ndb.btree;
using pst.encodables.ndb.maps;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ndb
{
    class AMapDecoder : IDecoder<AMap>
    {
        private readonly IDecoder<PageTrailer> pageTrailerDecoder;

        public AMapDecoder(IDecoder<PageTrailer> pageTrailerDecoder)
        {
            this.pageTrailerDecoder = pageTrailerDecoder;
        }

        public AMap Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new AMap(
                    parser.TakeAndSkip(496),
                    parser.TakeAndSkip(16, pageTrailerDecoder));
        }
    }
}
