using pst.interfaces.messaging.model;

namespace pst.interfaces.messaging
{
    interface IUnallocatedNodeIdGenerator
    {
        UnallocatedNodeId New();
    }
}