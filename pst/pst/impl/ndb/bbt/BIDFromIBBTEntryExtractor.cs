using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.bbt
{
    class BIDFromIBBTEntryExtractor : IExtractor<IBBTEntry, BID>
    {
        public BID Extract(IBBTEntry parameter)
        {
            return parameter.Key;
        }
    }
}
