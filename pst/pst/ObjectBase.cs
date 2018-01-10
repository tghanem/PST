using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.utilities;

namespace pst
{
    public abstract class ObjectBase
    {
        private readonly NodePath nodePath;
        private readonly IChangesTracker changesTracker;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;

        internal ObjectBase(
            NodePath nodePath,
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.changesTracker = changesTracker;
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

            changesTracker.SetProperty(nodePath, resolvedTag.Value, propertyValue);
        }

        public void SetProperty(StringPropertyTag propertyTag, PropertyValue propertyValue)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.SetProperty(nodePath, resolvedTag.Value, propertyValue);
        }

        public void SetProperty(PropertyTag propertyTag, PropertyValue propertyValue)
        {
            changesTracker.SetProperty(nodePath, propertyTag, propertyValue);
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
                changesTracker.GetProperty(
                    nodePath,
                    propertyTag,
                    () => propertyContextBasedPropertyReader.Read(nodePath.AllocatedIds, propertyTag));
        }

        public void DeleteProperty(NumericalPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.DeleteProperty(nodePath, resolvedTag.Value);
        }

        public void DeleteProperty(StringPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.DeleteProperty(nodePath, resolvedTag.Value);
        }

        public void DeleteProperty(PropertyTag propertyTag)
        {
            changesTracker.DeleteProperty(nodePath, propertyTag);
        }
    }
}
