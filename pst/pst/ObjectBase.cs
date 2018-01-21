using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.utilities;
using System.Linq;

namespace pst
{
    public abstract class ObjectBase
    {
        private readonly ObjectPath objectPath;
        private readonly IObjectTracker objectTracker;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;

        internal ObjectBase(
            ObjectPath objectPath,
            IObjectTracker objectTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader)
        {
            this.objectPath = objectPath;
            this.objectTracker = objectTracker;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public void SetProperty(NumericalPropertyTag propertyTag, PropertyValue propertyValue)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.SetProperty(objectPath, resolvedTag.Value, propertyValue);
        }

        public void SetProperty(StringPropertyTag propertyTag, PropertyValue propertyValue)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.SetProperty(objectPath, resolvedTag.Value, propertyValue);
        }

        public void SetProperty(PropertyTag propertyTag, PropertyValue propertyValue)
        {
            objectTracker.SetProperty(objectPath, propertyTag, propertyValue);
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(resolvedTag.Value);
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(resolvedTag.Value);
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                objectTracker.GetProperty(
                    objectPath,
                    propertyTag,
                    () => propertyContextBasedPropertyReader.Read(GetNodePath(), propertyTag));
        }

        public void DeleteProperty(NumericalPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.DeleteProperty(objectPath, resolvedTag.Value);
        }

        public void DeleteProperty(StringPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.DeleteProperty(objectPath, resolvedTag.Value);
        }

        public void DeleteProperty(PropertyTag propertyTag)
        {
            objectTracker.DeleteProperty(objectPath, propertyTag);
        }

        private NID[] GetNodePath()
        {
            if (objectPath.LocalNodeId.Type == Constants.NID_TYPE_NORMAL_FOLDER)
            {
                return new[] { objectPath.LocalNodeId };
            }

            return objectPath.Ids.Where(id => id.Type != Constants.NID_TYPE_NORMAL_FOLDER).ToArray();
        }
    }
}
