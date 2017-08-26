using pst.core;
using pst.encodables;
using pst.interfaces.messaging;

namespace pst
{
    public class Attachment
    {
        private readonly NodePath attachmentNodePath;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedReadOnlyComponent readOnlyComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Attachment(
            NodePath attachmentNodePath,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedReadOnlyComponent readOnlyComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.attachmentNodePath = attachmentNodePath;
            this.readOnlyMessage = readOnlyMessage;
            this.readOnlyComponent = readOnlyComponent;
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
                    readOnlyComponent, readOnlyComponentForRecipient);
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new NumericalTaggedPropertyPath(attachmentNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new StringTaggedPropertyPath(attachmentNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new TaggedPropertyPath(attachmentNodePath, propertyTag));
        }
    }
}
