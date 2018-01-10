using pst.interfaces.model;

namespace pst.interfaces.messaging
{
    interface IUnallocatedNodeIdGenerator
    {
        UnallocatedNodeId New();
    }
}