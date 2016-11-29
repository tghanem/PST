using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.bbt
{
    class BIDFromLBBTEntryExtractor : IExtractor<LBBTEntry, BID>
    {
        public BID Extract(LBBTEntry parameter)
        {
            return parameter.BlockReference.BlockId;
        }
    }
}
