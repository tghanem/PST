using pst.core;
using pst.impl.messaging.changetracking;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.model.changetracking;
using pst.utilities;

namespace pst
{
    public class Recipient
    {
        private readonly AssociatedObjectPath associatedObjectPath;
        private readonly IChangesTracker changesTracker;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;

        internal Recipient(
            AssociatedObjectPath associatedObjectPath,
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader)
        {
            this.associatedObjectPath = associatedObjectPath;
            this.changesTracker = changesTracker;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
        }

        public void SetProperty(NumericalPropertyTag propertyTag, PropertyValue propertyValue)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.SetProperty(associatedObjectPath, resolvedTag.Value, propertyValue);
        }

        public void SetProperty(StringPropertyTag propertyTag, PropertyValue propertyValue)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.SetProperty(associatedObjectPath, resolvedTag.Value, propertyValue);
        }

        public void SetProperty(PropertyTag propertyTag, PropertyValue propertyValue)
        {
            changesTracker.SetProperty(associatedObjectPath, propertyTag, propertyValue);
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
                    associatedObjectPath,
                    propertyTag,
                    () => tableContextBasedPropertyReader.Read(associatedObjectPath.NodePath.AllocatedIds, associatedObjectPath.RowId, propertyTag));
        }

        public void DeleteProperty(NumericalPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.DeleteProperty(associatedObjectPath, resolvedTag.Value);
        }

        public void DeleteProperty(StringPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            changesTracker.DeleteProperty(associatedObjectPath, resolvedTag.Value);
        }

        public void DeleteProperty(PropertyTag propertyTag)
        {
            changesTracker.DeleteProperty(associatedObjectPath, propertyTag);
        }
    }
}
