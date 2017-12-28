using pst.encodables.ndb;

namespace pst.interfaces.ndb.allocation
{
    interface IBlockIdGenerator
    {
        BID New();
    }
}