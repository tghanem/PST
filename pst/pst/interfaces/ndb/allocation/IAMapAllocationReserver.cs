using pst.encodables.ndb;

namespace pst.interfaces.ndb.allocation
{
    interface IAMapAllocationReserver
    {
        IB Reserve(AllocationInfo allocationInfo);
    }
}