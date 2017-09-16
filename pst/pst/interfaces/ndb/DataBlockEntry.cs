using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.interfaces.ndb
{
    class DataBlockEntry
    {
        public DataBlockEntry(LBBTEntry blockEntry, Maybe<BID[]> childBlockIds)
        {
            BlockEntry = blockEntry;
            ChildBlockIds = childBlockIds;
        }

        public LBBTEntry BlockEntry { get; }

        public Maybe<BID[]> ChildBlockIds { get; }
    }
}