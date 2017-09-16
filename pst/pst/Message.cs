using pst.core;
using pst.encodables;
using pst.interfaces.messaging;
using System.Linq;
using pst.interfaces.ndb;

namespace pst
{
    public class Message
    {
        private readonly NodePath messageNodePath;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedReadOnlyComponent readOnlyComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Message(
            NodePath messageNodePath,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedReadOnlyComponent readOnlyComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.messageNodePath = messageNodePath;
            this.readOnlyMessage = readOnlyMessage;
            this.readOnlyComponent = readOnlyComponent;
            this.readOnlyAttachment = readOnlyAttachment;
            this.readOnlyComponentForRecipient = readOnlyComponentForRecipient;
        }

        public Recipient[] GetRecipients()
        {
            var tagsForRecipients =
                readOnlyMessage.GetTagsForRecipients(messageNodePath);

            if (tagsForRecipients.HasNoValue)
            {
                return new Recipient[0];
            }

            var recipientTableNodeId =
                readOnlyMessage.GetRecipientTableNodeId(messageNodePath);

            if (recipientTableNodeId.HasNoValue)
            {
                return new Recipient[0];
            }

            var recipientTablePath =
                messageNodePath.Add(recipientTableNodeId.Value);

            return
                tagsForRecipients
                .Value
                .Select(tag => new Recipient(recipientTablePath, tag, readOnlyComponentForRecipient))
                .ToArray();
        }

        public Attachment[] GetAttachments()
        {
            var nidsForAttachments =
                readOnlyMessage.GetNodeIdsForAttachments(messageNodePath);

            if (nidsForAttachments.HasNoValue)
            {
                return new Attachment[0];
            }

            return
                nidsForAttachments
                .Value
                .Select(
                    nid =>
                    new Attachment(
                        messageNodePath.Add(nid),
                        readOnlyMessage,
                        readOnlyAttachment,
                        readOnlyComponent, readOnlyComponentForRecipient))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new NumericalTaggedPropertyPath(messageNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new StringTaggedPropertyPath(messageNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new TaggedPropertyPath(messageNodePath, propertyTag));
        }
    }
}
