using pst.core;
using pst.encodables.ltp.tc;
using pst.impl.messaging.changetracking;
using System;

namespace pst.interfaces.messaging.model.changetracking
{
    interface IChangesTracker
    {
        void TrackNode(
            NodePath nodePath,
            ObjectTypes objectType,
            ObjectStates objectState,
            Maybe<NodePath> parentNodePath);

        void Associate(
            NodePath nodePath,
            TCROWID associatedObjectTag,
            ObjectTypes associatedObjectType,
            ObjectStates associatdObjectState);

        bool IsObjectTracked(
            NodePath nodePath);

        AssociatedObjectPath[] GetAssociatedObjects(
            NodePath nodePath);

        NodePath[] GetChildren(
            NodePath parentNodePath,
            ObjectTypes childType,
            Predicate<ObjectStates> childStatePredicate);

        void SetProperty(
            NodePath nodePath,
            PropertyTag propertyTag,
            PropertyValue propertyValue);

        void SetProperty(
            AssociatedObjectPath path,
            PropertyTag propertyTag,
            PropertyValue propertyValue);

        void DeleteProperty(
            NodePath nodePath,
            PropertyTag propertyTag);

        void DeleteProperty(
            AssociatedObjectPath path,
            PropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(
            NodePath nodePath,
            PropertyTag propertyTag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader);

        Maybe<PropertyValue> GetProperty(
            AssociatedObjectPath path,
            PropertyTag propertyTag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader);
    }
}