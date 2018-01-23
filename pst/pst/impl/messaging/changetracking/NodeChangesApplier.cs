using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using System;
using pst.interfaces.messaging.changetracking.model;

namespace pst.impl.messaging.changetracking
{
    class NodeChangesApplier : INodeChangesApplier
    {
        public ChangesToCascade Apply(NodeTrackingObject trackingObject, Tuple<NodeTrackingObject, ChangesToCascade>[] changesToChildren)
        {
            throw new NotImplementedException();
        }
    }
}
