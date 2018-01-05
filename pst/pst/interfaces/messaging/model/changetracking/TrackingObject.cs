using pst.core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pst.interfaces.messaging.model.changetracking
{
    enum ObjectStates
    {
        PreExisting,
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
        PreExisting,
        New,
        Deleted
    }

    class TrackingObject
    {
        private Dictionary<PropertyTag, PropertyTrackingObject> taggedProperties;
        private Dictionary<StringPropertyTag, PropertyTrackingObject> stringTaggedProperties;
        private Dictionary<NumericalPropertyTag, PropertyTrackingObject> numericalTaggedProperties;

        public TrackingObject(NodePath path, ObjectTypes type, ObjectStates state, Maybe<NodePath> parentPath)
        {
            Path = path;
            Type = type;
            State = state;
            ParentPath = parentPath;

            taggedProperties = new Dictionary<PropertyTag, PropertyTrackingObject>();
            stringTaggedProperties = new Dictionary<StringPropertyTag, PropertyTrackingObject>();
            numericalTaggedProperties = new Dictionary<NumericalPropertyTag, PropertyTrackingObject>();
        }

        public NodePath Path { get; }

        public ObjectTypes Type { get; }

        public ObjectStates State { get; }

        public Maybe<NodePath> ParentPath { get; }

        public Tuple<PropertyTag, PropertyTrackingObject>[] GetChangedTaggedProperties()
        {
            return taggedProperties.Select(p => Tuple.Create(p.Key, p.Value)).ToArray();
        }

        public Tuple<StringPropertyTag, PropertyTrackingObject>[] GetChangedStringTaggedProperties()
        {
            return stringTaggedProperties.Select(p => Tuple.Create(p.Key, p.Value)).ToArray();
        }

        public Tuple<NumericalPropertyTag, PropertyTrackingObject>[] GetChangedNumericalTaggedProperties()
        {
            return numericalTaggedProperties.Select(p => Tuple.Create(p.Key, p.Value)).ToArray();
        }

        public Maybe<PropertyTrackingObject> ReadProperty(PropertyTag tag)
        {
            if (taggedProperties.ContainsKey(tag))
            {
                return taggedProperties[tag];
            }

            return Maybe<PropertyTrackingObject>.NoValue();
        }

        public Maybe<PropertyTrackingObject> ReadProperty(StringPropertyTag tag)
        {
            if (stringTaggedProperties.ContainsKey(tag))
            {
                return stringTaggedProperties[tag];
            }

            return Maybe<PropertyTrackingObject>.NoValue();
        }

        public Maybe<PropertyTrackingObject> ReadProperty(NumericalPropertyTag tag)
        {
            if (numericalTaggedProperties.ContainsKey(tag))
            {
                return numericalTaggedProperties[tag];
            }

            return Maybe<PropertyTrackingObject>.NoValue();
        }

        public void DeleteProperty(PropertyTag tag)
        {
            if (taggedProperties.ContainsKey(tag)) taggedProperties.Remove(tag);
        }

        public void DeleteProperty(StringPropertyTag tag)
        {
            if (stringTaggedProperties.ContainsKey(tag)) stringTaggedProperties.Remove(tag);
        }

        public void DeleteProperty(NumericalPropertyTag tag)
        {
            if (numericalTaggedProperties.ContainsKey(tag)) numericalTaggedProperties.Remove(tag);
        }

        public void UpdateProperty(PropertyTag tag, Func<Maybe<PropertyTrackingObject>, PropertyTrackingObject> update)
        {
            UpdateProperty(tag, taggedProperties, update);
        }

        public void UpdateProperty(StringPropertyTag tag, Func<Maybe<PropertyTrackingObject>, PropertyTrackingObject> update)
        {
            UpdateProperty(tag, stringTaggedProperties, update);
        }

        public void UpdateProperty(NumericalPropertyTag tag, Func<Maybe<PropertyTrackingObject>, PropertyTrackingObject> update)
        {
            UpdateProperty(tag, numericalTaggedProperties, update);
        }

        private void UpdateProperty<TPropertTag>(
            TPropertTag tag,
            Dictionary<TPropertTag, PropertyTrackingObject> cache,
            Func<Maybe<PropertyTrackingObject>, PropertyTrackingObject> update)
        {
            if (cache.ContainsKey(tag))
            {
                cache[tag] = update(cache[tag]);
            }
            else
            {
                cache.Add(tag, update(Maybe<PropertyTrackingObject>.NoValue()));
            }
        }
    }
}
