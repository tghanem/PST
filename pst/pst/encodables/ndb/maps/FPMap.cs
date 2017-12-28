using pst.encodables.ndb.btree;
using pst.utilities;

namespace pst.encodables.ndb.maps
{
    class FPMap
    {
        public FPMap(BinaryData data, PageTrailer pageTrailer)
        {
            Data = data;
            PageTrailer = pageTrailer;
        }

        public BinaryData Data { get; }

        public PageTrailer PageTrailer { get; }
    }
}
