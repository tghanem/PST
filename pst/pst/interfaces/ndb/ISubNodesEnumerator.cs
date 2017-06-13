using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;

namespace pst.interfaces.ndb
{
    interface ISubnodesEnumerator
    {
        SLEntry[] Enumerate(BID subnodeDataBlockId);
    }
}
