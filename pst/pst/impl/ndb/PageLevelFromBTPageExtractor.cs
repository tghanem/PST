using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb
{
    class PageLevelFromBTPageExtractor : IExtractor<BTPage, int>
    {
        public int Extract(BTPage parameter)
        {
            return parameter.PageLevel;
        }
    }
}
