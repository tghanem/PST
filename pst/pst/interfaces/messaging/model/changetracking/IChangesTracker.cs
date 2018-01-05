using pst.core;
using System;

namespace pst.interfaces.messaging.model.changetracking
{
    interface IChangesTracker
    {
        void TrackObject(
            NodePath path,
            ObjectTypes type,
            ObjectStates state,
            Maybe<NodePath> parentPath);

        void UpdateObject(NodePath path, Func<TrackingObject, TrackingObject> update);

        bool IsObjectTracked(NodePath path);

        T InspectObject<T>(NodePath path, Func<TrackingObject, T> inspect);
    }
}