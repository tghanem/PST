using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface IDataTreeLeafBIDsEnumerator
    {
        BID[] Enumerate(BID blockId);
    }
}
