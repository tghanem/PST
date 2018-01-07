using pst.core;
using System;

namespace pst.interfaces.messaging.model.changetracking
{
    interface IChangesTracker
    {
        void TrackObject(
            NodePath nodePath,
            ObjectTypes objectType,
            ObjectStates objectState,
            Maybe<NodePath> parentNodePath);

        void SetProperty(
            NodePath nodePath,
            PropertyTag propertyTag,
            PropertyValue propertyValue);

        void DeleteProperty(
            NodePath nodePath,
            PropertyTag propertyTag);

        bool IsObjectTracked(NodePath nodePath);

        NodePath[] GetChildren(NodePath parentNodePath, Predicate<ObjectStates> childStatePredicate);

        Maybe<PropertyValue> GetProperty(
            NodePath nodePath,
            PropertyTag propertyTag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader);
    }
}