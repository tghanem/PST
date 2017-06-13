using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface IDataTreeBlockLevelDecider
    {
        int GetBlockLevel(BID blockId);
    }
}
