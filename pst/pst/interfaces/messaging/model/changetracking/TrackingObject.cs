using pst.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.interfaces.messaging.model.changetracking
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

        public TrackingObject(NodePath path, ObjectTypes type, ObjectStates state, Maybe<NodePath> parentPath)
        {
            Path = path;
            Type = type;
            State = state;
            ParentPath = parentPath;

            properties = new Dictionary<PropertyTag, PropertyTrackingObject>();
        }

        public NodePath Path { get; }

        public ObjectTypes Type { get; }

        public ObjectStates State { get; }

        public Maybe<NodePath> ParentPath { get; }

        public Tuple<PropertyTag, PropertyTrackingObject>[] Properties => properties.Select(p => Tuple.Create(p.Key, p.Value)).ToArray();

        public Maybe<PropertyTrackingObject> ReadProperty(PropertyTag tag)
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
}
