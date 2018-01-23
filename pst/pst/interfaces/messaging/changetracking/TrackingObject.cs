using pst.core;
using System;
using System.Collections.Generic;

namespace pst.interfaces.messaging.changetracking
{
    enum ObjectStates
    {
        Loaded,
        New,
        Deleted
    }

    enum ObjectTypes
    {
        Store,
        Folder,
        Message,
        Recipient,
        RecipientTable,
        Attachment
    }

    enum PropertyStates
    {
        Loaded,
        LoadedAndUpdated,
        LoadedAndDeleted,
        New,
        Deleted
    }

    class TrackingObject
    {
        private readonly List<PropertyTrackingObject> properties;

        public TrackingObject(ObjectTypes type, ObjectStates state)
        {
            Type = type;
            State = state;

            properties = new List<PropertyTrackingObject>();
        }

        public ObjectTypes Type { get; }

        public ObjectStates State { get; }

        public PropertyTrackingObject[] Properties => properties.ToArray();

        public Maybe<PropertyTrackingObject> GetProperty(PropertyTag tag) => properties.Find(p => p.Tag.Equals(tag));

        public void UpdateProperty(PropertyTag tag, Func<Maybe<PropertyTrackingObject>, PropertyTrackingObject> update)
        {
            var property = properties.Find(p => p.Tag.Equals(tag));

            if (property != null)
            {
                properties.Remove(property);
                properties.Add(update(property));
            }

            properties.Add(update(Maybe<PropertyTrackingObject>.NoValue()));
        }
    }
}

