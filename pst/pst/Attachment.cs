using pst.core;
using pst.encodables;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

namespace pst
{
    public class Attachment
    {
        private readonly NodePath attachmentNodePath;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedComponent propertyContextBasedComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Attachment(
            NodePath attachmentNodePath,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedComponent propertyContextBasedComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.attachmentNodePath = attachmentNodePath;
            this.readOnlyMessage = readOnlyMessage;
            this.propertyContextBasedComponent = propertyContextBasedComponent;
            this.readOnlyAttachment = readOnlyAttachment;
            this.readOnlyComponentForRecipient = readOnlyComponentForRecipient;
        }

        public Maybe<Message> GetEmbeddedMessage()
        {
            var embeddedMessageNID =
                readOnlyAttachment.GetEmbeddedMessageNodeId(attachmentNodePath);

            if (embeddedMessageNID.HasNoValue)
            {
                return Maybe<Message>.NoValue();
            }

            return
                new Message(
                    attachmentNodePath.Add(embeddedMessageNID.Value),
                    readOnlyMessage,
                    readOnlyAttachment,
                    propertyContextBasedComponent, readOnlyComponentForRecipient);
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new NumericalTaggedPropertyPath(attachmentNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new StringTaggedPropertyPath(attachmentNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new TaggedPropertyPath(attachmentNodePath, propertyTag));
        }
    }
}
