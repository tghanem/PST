using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.bbt
{
    class BREFFromIBBTEntryExtractor : IExtractor<IBBTEntry, BREF>
    {
        public BREF Extract(IBBTEntry parameter)
        {
            return parameter.ChildPageBlockReference;
        }
    }
}
