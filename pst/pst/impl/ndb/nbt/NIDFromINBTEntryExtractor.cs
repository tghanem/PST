using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.nbt
{
    class NIDFromINBTEntryExtractor : IExtractor<INBTEntry, NID>
    {
        public NID Extract(INBTEntry parameter)
        {
            return parameter.Key;
        }
    }
}
