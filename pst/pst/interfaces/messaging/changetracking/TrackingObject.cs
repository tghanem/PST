using pst.core;
using pst.interfaces.model;
using System;
using System.Collections.Generic;
using pst.encodables.ndb;

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

    class NodeTrackingObject : TrackingObject
    {
        private readonly List<NodeTrackingObject> children;

        public NodeTrackingObject(ObjectPath path, ObjectTypes type, ObjectStates state) : base(type, state)
        {
            Path = path;
            children = new List<NodeTrackingObject>();
        }

        public ObjectPath Path { get; }

        public NodeTrackingObject[] Children => children.ToArray();

        public void AddChild(NodeTrackingObject child) => children.Add(child);
    }

    class RecipientTableTrackingObject : TrackingObject
    {
        public RecipientTableTrackingObject(NID recipientTableNodeId, ObjectTypes type, ObjectStates state) : base(type, state)
        {
            RecipientTableNodeId = recipientTableNodeId;
            TrackedRecipients = new Dictionary<int, TrackingObject>();
        }

        public NID RecipientTableNodeId { get; }

        public IDictionary<int, TrackingObject> TrackedRecipients { get; }
    }
}

