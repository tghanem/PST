using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.nbt
{
    class NIDFromLNBTEntryExtractor : IExtractor<LNBTEntry, NID>
    {
        public NID Extract(LNBTEntry parameter)
        {
            return parameter.NodeId;
        }
    }
}
