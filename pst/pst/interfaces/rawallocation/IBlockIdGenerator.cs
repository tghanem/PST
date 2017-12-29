using pst.encodables.ndb;

namespace pst.interfaces.rawallocation
{
    interface IBlockIdGenerator
    {
        BID New();
    }
}