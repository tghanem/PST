using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;

namespace pst.interfaces.blockallocation.subnodebtree
{
    interface ISubnodeBTreeAllocator
    {
        BID Allocate(SLEntry[] entriesForSubnodes);
    }
}