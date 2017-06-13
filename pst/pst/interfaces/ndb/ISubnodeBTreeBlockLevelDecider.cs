using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface ISubnodeBTreeBlockLevelDecider
    {
        int GetBlockLevel(BID blockId);
    }
}
