using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using System.Collections.Generic;

namespace pst.impl.messaging.changetracking
{
    class ChangesApplier : IChangesApplier
    {
        private readonly IDictionary<ObjectPath, NodeTrackingObject> trackedNodes;

        public ChangesApplier(IDictionary<ObjectPath, NodeTrackingObject> trackedNodes)
        {
            this.trackedNodes = trackedNodes;
        }

        public void Apply()
        {
            foreach (var rootTrackedNode in trackedNodes.Values)
            {
                Apply(rootTrackedNode);
            }
        }

        private void Apply(NodeTrackingObject trackingObject)
        {
            foreach (var child in trackingObject.Children)
            {
                Apply(child);

                if (child.Type == ObjectTypes.Folder)
                {
                    
                }
            }
        }
    }
}
