using pst.core;
using pst.encodables;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.utilities;

namespace pst
{
    public class Recipient
    {
        private readonly NodePath nodePath;
        private readonly Tag recipientRowId;
        private readonly IChangesTracker changesTracker;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader;

        internal Recipient(
            NodePath nodePath,
            Tag recipientRowId,
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.recipientRowId = recipientRowId;
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
                    nodePath, 
                    propertyTag,
                    () => tableContextBasedPropertyReader.Read(nodePath.AllocatedIds, recipientRowId, propertyTag));
        }
    }
}
