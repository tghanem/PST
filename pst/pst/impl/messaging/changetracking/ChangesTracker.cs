using pst.core;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using System;
using System.Collections.Generic;

namespace pst.impl.messaging.changetracking
{
    class ChangesTracker : IChangesTracker
    {
        private readonly Dictionary<NodePath, TrackingObject> trackedObjects;

        public ChangesTracker()
        {
            trackedObjects = new Dictionary<NodePath, TrackingObject>();
        }

        public void TrackObject(
            NodePath path,
            ObjectTypes type,
            ObjectStates state,
            Maybe<NodePath> parentPath)
        {
            trackedObjects.Add(path, new TrackingObject(path, type, state, parentPath));
        }

        public void UpdateObject(NodePath path, Func<TrackingObject, TrackingObject> update)
        {
            trackedObjects[path] = update(trackedObjects[path]);
        }

        public bool IsObjectTracked(NodePath path)
        {
            return trackedObjects.ContainsKey(path);
        }

        public T InspectObject<T>(NodePath path, Func<TrackingObject, T> inspect)
        {
            return inspect(trackedObjects[path]);
        }
    }
}
