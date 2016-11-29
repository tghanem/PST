using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.nbt
{
    class BREFFromINBTEntryExtractor : IExtractor<INBTEntry, BREF>
    {
        public BREF Extract(INBTEntry parameter)
        {
            return parameter.ChildPageBlockReference;
        }
    }
}
