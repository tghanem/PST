using pst.core;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.impl.messaging.changetracking
{
    class ObjectTracker : IObjectTracker
    {
        private readonly IDictionary<ObjectPath, NodeTrackingObject> rootTrackingObjects;

        public ObjectTracker(IDictionary<ObjectPath, NodeTrackingObject> rootTrackingObjects)
        {
            this.rootTrackingObjects = rootTrackingObjects;
        }

        public void TrackObject(ObjectPath objectPath, ObjectTypes objectType, ObjectStates objectState)
        {
            var trackingObject = new NodeTrackingObject(objectPath, objectType, objectState);

            if (!objectPath.HasParent)
            {
                rootTrackingObjects.Add(objectPath, trackingObject);
            }
            else
            {
                GetTrackingObject(objectPath.ParentObjectPath).AddChild(trackingObject);
            }
        }

        public bool IsObjectTracked(ObjectPath objectPath)
        {
            if (!objectPath.HasParent)
            {
                return rootTrackingObjects.ContainsKey(objectPath);
            }

            return GetTrackingObject(objectPath).Children.Any(c => c.Path.Equals(objectPath));
        }

        public ObjectPath[] GetChildObjects(ObjectPath objectPath, ObjectTypes childType, Predicate<ObjectStates> childStatePredicate)
        {
            return GetTrackingObject(objectPath).Children.Where(c => c.Type == childType && childStatePredicate(c.State)).Select(c => c.Path).ToArray();
        }

        public void SetProperty(ObjectPath objectPath, PropertyTag propertyTag, PropertyValue propertyValue)
        {
            GetTrackingObject(objectPath).UpdateProperty(propertyTag, propertyTrackingObject => propertyTrackingObject.SetProperty(propertyTag, propertyValue));
        }

        public void DeleteProperty(ObjectPath objectPath, PropertyTag propertyTag)
        {
            GetTrackingObject(objectPath).UpdateProperty(propertyTag, trackingObject => trackingObject.DeleteProperty(propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(ObjectPath objectPath, PropertyTag propertyTag, Func<Maybe<PropertyValue>> untrackedPropertyValueReader)
        {
            return GetTrackingObject(objectPath).GetProperty(propertyTag, untrackedPropertyValueReader);
        }

        private NodeTrackingObject GetTrackingObject(ObjectPath objectPath)
        {
            if (!objectPath.HasParent)
            {
                return rootTrackingObjects[objectPath];
            }

            var rootTrackingObject = rootTrackingObjects[objectPath.RootObjectPath];

            return GetTrackingObject(objectPath, rootTrackingObject, 1);
        }

        private NodeTrackingObject GetTrackingObject(ObjectPath objectPath, NodeTrackingObject currentTrackingObject, int depth)
        {
            if (depth == objectPath.Ids.Length)
            {
                return currentTrackingObject;
            }

            for (var i = 0; i < currentTrackingObject.Children.Length; i++)
            {
                if (currentTrackingObject.Children[i].Path.LocalNodeId.Equals(objectPath.Ids[depth]))
                {
                    return GetTrackingObject(objectPath, currentTrackingObject.Children[i], depth + 1);
                }
            }

            throw new Exception("Could not find tracking object");
        }
    }
}
