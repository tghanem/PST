using pst.encodables.ndb.btree;
using pst.encodables.ndb.maps;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.encoders.ndb.maps
{
    class MapEncoder : IEncoder<AMap>, IEncoder<PMap>, IEncoder<FMap>, IEncoder<FPMap>
    {
        private readonly IEncoder<PageTrailer> pageTrailerEncoder;

        public MapEncoder(IEncoder<PageTrailer> pageTrailerEncoder)
        {
            this.pageTrailerEncoder = pageTrailerEncoder;
        }

        public BinaryData Encode(AMap value)
        {
            return Encode(value.Data, value.PageTrailer);
        }

        public BinaryData Encode(PMap value)
        {
            return Encode(value.Data, value.PageTrailer);
        }

        public BinaryData Encode(FMap value)
        {
            return Encode(value.Data, value.PageTrailer);
        }

        public BinaryData Encode(FPMap value)
        {
            return Encode(value.Data, value.PageTrailer);
        }

        private BinaryData Encode(BinaryData data, PageTrailer pageTrailer)
        {
            var dataGenerator = BinaryDataGenerator.New();

            dataGenerator.Append(data);
            dataGenerator.Append(pageTrailer, pageTrailerEncoder);

            return dataGenerator.GetData();
        }
    }
}
