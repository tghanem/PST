using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.ltp;

namespace pst
{
    public class Recipient
    {
        private readonly BID recipientTableBlockId;
        private readonly BID recipientTableSubnodeBlockId;
        private readonly Tag recipientRowId;

        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader;

        internal Recipient(
            BID recipientTableBlockId,
            BID recipientTableSubnodeBlockId,
            Tag recipientRowId,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader)
        {
            this.recipientTableBlockId = recipientTableBlockId;
            this.recipientTableSubnodeBlockId = recipientTableSubnodeBlockId;
            this.recipientRowId = recipientRowId;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                tableContextBasedPropertyReader.ReadProperty(
                    recipientTableBlockId,
                    recipientTableSubnodeBlockId,
                    recipientRowId,
                    propertyTag);
        }
    }
}
