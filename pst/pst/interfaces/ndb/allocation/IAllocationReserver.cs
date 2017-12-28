using pst.encodables.ndb;

namespace pst.interfaces.ndb.allocation
{
    interface IAllocationReserver
    {
        IB Reserve(AllocationInfo allocationInfo);
    }
}