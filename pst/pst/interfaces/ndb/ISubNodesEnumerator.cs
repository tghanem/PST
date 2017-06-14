using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;

namespace pst.interfaces.ndb
{
    interface ISubNodesEnumerator
    {
        SLEntry[] Enumerate(BID subnodeDataBlockId);
    }
}
