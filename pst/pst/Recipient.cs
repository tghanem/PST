using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.utilities;

namespace pst
{
    public class Recipient
    {
        private readonly ObjectPath messageObjectPath;
        private readonly NID recipientTableNodeId;
        private readonly int recipientRowId;
        private readonly IRecipientTracker objectTracker;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;

        internal Recipient(
            ObjectPath messageObjectPath,
            NID recipientTableNodeId,
            int recipientRowId,
            IRecipientTracker objectTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader)
        {
            this.messageObjectPath = messageObjectPath;
            this.recipientTableNodeId = recipientTableNodeId;
            this.recipientRowId = recipientRowId;
            this.objectTracker = objectTracker;
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

            objectTracker.SetProperty(
                messageObjectPath,
                recipientTableNodeId,
                recipientRowId,
                resolvedTag.Value,
                propertyValue);
        }

        public void SetProperty(StringPropertyTag propertyTag, PropertyValue propertyValue)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.SetProperty(
                messageObjectPath,
                recipientTableNodeId,
                recipientRowId,
                resolvedTag.Value,
                propertyValue);
        }

        public void SetProperty(PropertyTag propertyTag, PropertyValue propertyValue)
        {
            objectTracker.SetProperty(
                messageObjectPath,
                recipientTableNodeId,
                recipientRowId,
                propertyTag,
                propertyValue);
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
                    messageObjectPath,
                    recipientTableNodeId,
                    recipientRowId,
                    propertyTag,
                    () => tableContextBasedPropertyReader.Read(new[] { messageObjectPath.LocalNodeId, recipientTableNodeId }, recipientRowId, propertyTag));
        }

        public void DeleteProperty(NumericalPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.DeleteProperty(
                messageObjectPath,
                recipientTableNodeId,
                recipientRowId,
                resolvedTag.Value);
        }

        public void DeleteProperty(StringPropertyTag propertyTag)
        {
            var resolvedTag = propertyNameToIdMap.Resolve(propertyTag);

            if (resolvedTag.HasNoValue)
            {
                return;
            }

            objectTracker.DeleteProperty(
                messageObjectPath,
                recipientTableNodeId,
                recipientRowId,
                resolvedTag.Value);
        }

        public void DeleteProperty(PropertyTag propertyTag)
        {
            objectTracker.DeleteProperty(
                messageObjectPath,
                recipientTableNodeId,
                recipientRowId,
                propertyTag);
        }
    }
}
