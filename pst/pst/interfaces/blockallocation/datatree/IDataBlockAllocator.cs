using pst.encodables.ndb;

namespace pst.interfaces.blockallocation.datatree
{
    interface IDataBlockAllocator<TValue>
    {
        BID Allocate(TValue rawValue);
    }
}