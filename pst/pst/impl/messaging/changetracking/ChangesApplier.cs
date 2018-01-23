using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.messaging.changetracking.model;
using System;
using System.Collections.Generic;

namespace pst.impl.messaging.changetracking
{
    class ChangesApplier : IChangesApplier
    {
        private readonly IDictionary<ObjectPath, NodeTrackingObject> trackedNodes;
        private readonly INodeChangesApplier nodeChangesApplier;

        public ChangesApplier(IDictionary<ObjectPath, NodeTrackingObject> trackedNodes, INodeChangesApplier nodeChangesApplier)
        {
            this.trackedNodes = trackedNodes;
            this.nodeChangesApplier = nodeChangesApplier;
        }

        public void Apply()
        {
            foreach (var rootTrackedNode in trackedNodes.Values)
            {
                Apply(rootTrackedNode);
            }
        }

        private ChangesToCascade Apply(NodeTrackingObject trackingObject)
        {
            var changesToChildren = new List<Tuple<NodeTrackingObject, ChangesToCascade>>();

            foreach (var child in trackingObject.Children)
            {
                var changesToCascade = Apply(child);

                changesToChildren.Add(Tuple.Create(child, changesToCascade));
            }

            return nodeChangesApplier.Apply(trackingObject, changesToChildren.ToArray());
        }
    }
}
