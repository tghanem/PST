using pst.encodables.ndb;

namespace pst.interfaces.blockallocation.datatree
{
    interface IDataBlockAllocator<TValue>
    {
        BREF Allocate(TValue rawValue);
    }
}