using pst.core;
using pst.encodables;
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
        private readonly ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader;

        internal Recipient(
            AssociatedObjectPath associatedObjectPath,
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader)
        {
            this.associatedObjectPath = associatedObjectPath;
            this.changesTracker = changesTracker;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
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
                    () => tableContextBasedPropertyReader.Read(associatedObjectPath.NodePath.AllocatedIds, associatedObjectPath.Tag, propertyTag));
        }
    }
}
