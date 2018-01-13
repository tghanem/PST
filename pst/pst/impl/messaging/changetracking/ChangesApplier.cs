using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using System;
using System.Collections.Generic;

namespace pst.impl.messaging.changetracking
{
    class ChangesApplier : IChangesApplier
    {
        private readonly IDictionary<NodePath, NodeTrackingObject> trackedObjects;
        private readonly IDictionary<AssociatedObjectPath, TrackingObject> associatedObjects;
        private readonly ITableContextReader tableContextReader;

        public ChangesApplier(
            IDictionary<NodePath, NodeTrackingObject> trackedObjects,
            IDictionary<AssociatedObjectPath, TrackingObject> associatedObjects,
            ITableContextReader tableContextReader)
        {
            this.trackedObjects = trackedObjects;
            this.associatedObjects = associatedObjects;
            this.tableContextReader = tableContextReader;
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
