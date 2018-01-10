using pst.interfaces.messaging;
using pst.interfaces.model;

namespace pst.impl.messaging
{
    class UnallocatedNodeIdGenerator : IUnallocatedNodeIdGenerator
    {
        private int index;

        public UnallocatedNodeId New()
        {
            return UnallocatedNodeId.OfValue(index++);
        }
    }
}
