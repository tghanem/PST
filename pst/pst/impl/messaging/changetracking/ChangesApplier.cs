using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.impl.messaging.changetracking
{
    class ChangesApplier : IChangesApplier
    {
        private readonly IDictionary<NodePath, NodeTrackingObject> trackedObjects;
        private readonly IDictionary<AssociatedObjectPath, TrackingObject> associatedObjects;

        private readonly IEncoder<NID> nidEncoder;
        private readonly IFolderGenerator folderGenerator;
        private readonly IExtractor<NodeTrackingObject, Tuple<PropertyTag, PropertyValue>[]> childFolderPropertiesExtractor;

        public ChangesApplier(
            IDictionary<NodePath, NodeTrackingObject> trackedObjects,
            IDictionary<AssociatedObjectPath, TrackingObject> associatedObjects)
        {
            this.trackedObjects = trackedObjects;
            this.associatedObjects = associatedObjects;
        }

        public void Apply()
        {
            var rootObjects =
                trackedObjects
                .Values
                .Where(o => o.ParentPath.HasNoValue)
                .ToArray();

            foreach (var @object in rootObjects)
            {
                if (@object.Type == ObjectTypes.Folder)
                {
                    ApplyFolderChanges(@object);
                }
            }
        }

        private void ApplyFolderChanges(NodeTrackingObject @object)
        {
            var childFolders =
                trackedObjects
                .Values
                .Where(o => o.Type == ObjectTypes.Folder && o.ParentPath.HasValueAnd(p => p.Equals(@object.Path)))
                .ToArray();

            var rowsToAddToHierachyTable = new List<TableRow>();

            foreach (var child in childFolders)
            {
                if (child.State == ObjectStates.New)
                {
                    var folderNodeId = folderGenerator.Generate(child);
                }
            }
        }
    }
}
