using pst.core;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using System;
using System.Collections.Generic;
using System.Linq;

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
            NodePath nodePath,
            ObjectTypes objectType,
            ObjectStates objectState,
            Maybe<NodePath> parentNodePath)
        {
            trackedObjects.Add(
                nodePath,
                new TrackingObject(nodePath, objectType, objectState, parentNodePath));
        }

        public void SetProperty(
            NodePath nodePath,
            PropertyTag propertyTag,
            PropertyValue propertyValue)
        {
            trackedObjects[nodePath].UpdateProperty(
                propertyTag,
                propertyTrackingObject => SetProperty(propertyValue, propertyTrackingObject));
        }

        public void DeleteProperty(
            NodePath nodePath,
            PropertyTag propertyTag)
        {
            trackedObjects[nodePath].UpdateProperty(propertyTag, DeleteProperty);
        }

        public bool IsObjectTracked(NodePath nodePath)
        {
            return trackedObjects.ContainsKey(nodePath);
        }

        public NodePath[] GetChildren(NodePath parentNodePath, Predicate<ObjectStates> childStatePredicate)
        {
            return
                trackedObjects
                .Values
                .Where(o => o.ParentPath.HasValueAnd(p => p.Equals(parentNodePath)) && childStatePredicate(o.State))
                .Select(o => o.Path)
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(
            NodePath nodePath,
            PropertyTag propertyTag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader)
        {
            var propertyValue = GetProperty(trackedObjects[nodePath], propertyTag);

            if (propertyValue.HasValue)
            {
                return propertyValue;
            }

            var untrackedPropertyValue = untrackedPropertyValueReader();

            if (untrackedPropertyValue.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            trackedObjects[nodePath].UpdateProperty(
                propertyTag,
                p => new PropertyTrackingObject(PropertyStates.Loaded, untrackedPropertyValue.Value));

            return untrackedPropertyValue;
        }

        private Maybe<PropertyValue> GetProperty(TrackingObject trackingObject, PropertyTag propertyTag)
        {
            var property = trackingObject.ReadProperty(propertyTag);

            if (property.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            if (property.Value.State == PropertyStates.Deleted ||
                property.Value.State == PropertyStates.LoadedAndDeleted)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return property.Value.Value;
        }

        private PropertyTrackingObject SetProperty(PropertyValue propertyValue, Maybe<PropertyTrackingObject> propertyTrackingObject)
        {
            if (propertyTrackingObject.HasNoValue)
            {
                return new PropertyTrackingObject(PropertyStates.New, propertyValue);
            }

            if (propertyTrackingObject.Value.State == PropertyStates.Loaded ||
                propertyTrackingObject.Value.State == PropertyStates.LoadedAndUpdated ||
                propertyTrackingObject.Value.State == PropertyStates.LoadedAndDeleted)
            {
                return
                    propertyTrackingObject
                    .Value
                    .SetState(PropertyStates.LoadedAndUpdated)
                    .SetValue(propertyValue);
            }

            if (propertyTrackingObject.Value.State == PropertyStates.Deleted)
            {
                return new PropertyTrackingObject(PropertyStates.New, propertyValue);
            }

            return propertyTrackingObject.Value.SetValue(propertyValue);
        }

        private PropertyTrackingObject DeleteProperty(Maybe<PropertyTrackingObject> propertyTrackingObject)
        {
            if (propertyTrackingObject.HasValue)
            {
                if (propertyTrackingObject.Value.State == PropertyStates.Loaded ||
                    propertyTrackingObject.Value.State == PropertyStates.LoadedAndUpdated ||
                    propertyTrackingObject.Value.State == PropertyStates.LoadedAndDeleted)
                {
                    return propertyTrackingObject.Value.SetState(PropertyStates.LoadedAndDeleted);
                }

                return propertyTrackingObject.Value.SetState(PropertyStates.Deleted);
            }

            return new PropertyTrackingObject(PropertyStates.Deleted, Maybe<PropertyValue>.NoValue());
        }
    }
}
