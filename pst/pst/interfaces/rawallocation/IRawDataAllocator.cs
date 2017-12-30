using pst.encodables.ndb;

namespace pst.interfaces.rawallocation
{
    interface IRawDataAllocator
    {
        IB Allocate(int sizeOfDataInBytes);
    }
}