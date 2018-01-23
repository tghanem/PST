using pst.core;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.messaging.changetracking.model;
using System;

namespace pst.interfaces.messaging
{
    class ChangesToCascade
    {
        public ChangesToCascade(Maybe<PropertyTrackingObject[]> propertiesToCascadeToParent)
        {
            PropertiesToCascadeToParent = propertiesToCascadeToParent;
        }

        public Maybe<PropertyTrackingObject[]> PropertiesToCascadeToParent { get; }
    }

    interface INodeChangesApplier
    {
        ChangesToCascade Apply(NodeTrackingObject trackingObject, Tuple<NodeTrackingObject, ChangesToCascade>[] changesToChildren);
    }
}