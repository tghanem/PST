using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;

namespace pst
{
    public class Recipient
    {
        private readonly BID recipientTableBlockId;
        private readonly BID recipientTableSubnodeBlockId;
        private readonly Tag recipientRowId;

        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextReader<Tag> tableContextReader;

        internal Recipient(
            BID recipientTableBlockId,
            BID recipientTableSubnodeBlockId,
            Tag recipientRowId,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextReader<Tag> tableContextReader)
        {
            this.recipientTableBlockId = recipientTableBlockId;
            this.recipientTableSubnodeBlockId = recipientTableSubnodeBlockId;
            this.recipientRowId = recipientRowId;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.tableContextReader = tableContextReader;
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
                tableContextReader.ReadProperty(
                    recipientTableBlockId,
                    recipientTableSubnodeBlockId,
                    recipientRowId,
                    propertyTag);
        }
    }
}
