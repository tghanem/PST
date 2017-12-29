using pst.encodables.ndb;

namespace pst.interfaces.rawallocation
{
    interface IAllocationReserver
    {
        IB Reserve(AllocationInfo allocationInfo);
    }
}