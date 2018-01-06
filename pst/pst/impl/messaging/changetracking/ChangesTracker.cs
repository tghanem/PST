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

        public Maybe<PropertyValue> ReadProperty(
            NodePath nodePath,
            ObjectTypes objectType,
            PropertyTag tag,
            Func<Maybe<PropertyValue>> untrackedPropertyValueReader)
        {
            if (!trackedObjects.ContainsKey(nodePath))
            {
                trackedObjects.Add(nodePath, new TrackingObject(nodePath, objectType, ObjectStates.PreExisting, Maybe<NodePath>.NoValue()));
            }

            var propertyValue = ReadPropertyValue(trackedObjects[nodePath], tag);

            if (propertyValue.HasValue)
            {
                return propertyValue;
            }

            var untrackedPropertyValue = untrackedPropertyValueReader();

            if (untrackedPropertyValue.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            trackedObjects[nodePath].UpdateProperty(tag, p => new PropertyTrackingObject(PropertyStates.PreExisting, untrackedPropertyValue.Value));

            return untrackedPropertyValue;
        }

        private Maybe<PropertyValue> ReadPropertyValue(TrackingObject trackingObject, PropertyTag propertyTag)
        {
            var property = trackingObject.ReadProperty(propertyTag);

            if (property.HasValue)
            {
                if (property.Value.State == PropertyStates.Deleted)
                {
                    return Maybe<PropertyValue>.NoValue();
                }

                return property.Value.Value;
            }

            return Maybe<PropertyValue>.NoValue();
        }
    }
}
