using pst.core;
using pst.encodables;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

namespace pst
{
    public class Recipient
    {
        private readonly NodePath recipientTableSubnodePath;
        private readonly Tag recipientRowId;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponent;

        internal Recipient(
            NodePath recipientTableSubnodePath,
            Tag recipientRowId,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponent)
        {
            this.recipientRowId = recipientRowId;
            this.recipientTableSubnodePath = recipientTableSubnodePath;
            this.readOnlyComponent = readOnlyComponent;
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(recipientRowId, new NumericalTaggedPropertyPath(recipientTableSubnodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(recipientRowId, new StringTaggedPropertyPath(recipientTableSubnodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(recipientRowId, new TaggedPropertyPath(recipientTableSubnodePath, propertyTag));
        }
    }
}
