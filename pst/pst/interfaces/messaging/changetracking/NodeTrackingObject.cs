using pst.core;
using pst.interfaces.model;
using System;
using System.Collections.Generic;
using System.Linq;

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
        RecipientTable,
        Recipient,
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
        private readonly Dictionary<PropertyTag, PropertyTrackingObject> properties;

        public TrackingObject(ObjectTypes type, ObjectStates state)
        {
            Type = type;
            State = state;

            properties = new Dictionary<PropertyTag, PropertyTrackingObject>();
        }

        public ObjectTypes Type { get; }

        public ObjectStates State { get; }

        public Tuple<PropertyTag, PropertyTrackingObject>[] Properties => properties.Select(p => Tuple.Create(p.Key, p.Value)).ToArray();

        public Maybe<PropertyTrackingObject> GetProperty(PropertyTag tag)
        {
            if (properties.ContainsKey(tag))
            {
                return properties[tag];
            }

            return Maybe<PropertyTrackingObject>.NoValue();
        }

        public void UpdateProperty(PropertyTag tag, Func<Maybe<PropertyTrackingObject>, PropertyTrackingObject> update)
        {
            if (properties.ContainsKey(tag))
            {
                properties[tag] = update(properties[tag]);
            }
            else
            {
                properties.Add(tag, update(Maybe<PropertyTrackingObject>.NoValue()));
            }
        }
    }

    class NodeTrackingObject : TrackingObject
    {
        public NodeTrackingObject(NodePath path, ObjectTypes type, ObjectStates state, Maybe<NodePath> parentPath) : base(type, state)
        {
            Path = path;
            ParentPath = parentPath;
        }

        public NodePath Path { get; }

        public Maybe<NodePath> ParentPath { get; }
    }
}
