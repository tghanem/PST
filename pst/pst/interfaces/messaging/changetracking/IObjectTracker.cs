using pst.core;
using pst.interfaces.messaging.changetracking.model;
using System;

namespace pst.interfaces.messaging.changetracking
{
    interface IObjectTracker
    {
        void TrackObject(
            ObjectPath objectPath,
            ObjectTypes objectType,
            ObjectStates objectState);

        bool IsObjectTracked(
            ObjectPath objectPath);

        ObjectPath[] GetChildObjects(
            ObjectPath objectPath,
            ObjectTypes childType,
            Predicate<ObjectStates> childStatePredicate);

        void SetProperty(
            ObjectPath objectPath,
            PropertyTag propertyTag,
            PropertyValue propertyValue);

        void DeleteProperty(
            ObjectPath objectPath,
            PropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(
            ObjectPath objectPath,
            PropertyTag propertyTag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader);
    }
}