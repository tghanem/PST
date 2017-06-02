using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.interfaces.ndb
{
    interface IDataTreeLeafNodesEnumerator
    {
        BID[] Enumerate(LBBTEntry blockEntry);
    }
}
