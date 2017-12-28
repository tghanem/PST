using pst.encodables.ndb.btree;
using pst.utilities;

namespace pst.encodables.ndb.maps
{
    class AMap
    {
        public AMap(BinaryData data, PageTrailer pageTrailer)
        {
            Data = data;
            PageTrailer = pageTrailer;
        }

        public BinaryData Data { get; }

        public PageTrailer PageTrailer { get; }
    }
}
