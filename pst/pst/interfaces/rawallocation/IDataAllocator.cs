using pst.encodables.ndb;

namespace pst.interfaces.rawallocation
{
    interface IDataAllocator
    {
        IB Allocate(int sizeOfDataInBytes);
    }
}