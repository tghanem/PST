using pst.encodables.ndb;

namespace pst.interfaces.ndb.allocation
{
    interface IDataAllocator
    {
        IB Allocate(int sizeOfDataInBytes);
    }
}