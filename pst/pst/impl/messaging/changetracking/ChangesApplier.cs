using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using System;
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
            throw new NotImplementedException();
        }
    }
}
