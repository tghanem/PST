using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.messaging;
using System.Linq;

namespace pst
{
    public class Message
    {
        private readonly NID[] messageNodePath;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedReadOnlyComponent readOnlyComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Message(
            NID[] messageNodePath,
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
                messageNodePath.Concat(new[] { recipientTableNodeId.Value }).ToArray();

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
                        messageNodePath.Concat(new[] { nid }).ToArray(),
                        readOnlyMessage,
                        readOnlyAttachment,
                        readOnlyComponent, readOnlyComponentForRecipient))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(messageNodePath, propertyTag);
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(messageNodePath, propertyTag);
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(messageNodePath, propertyTag);
        }
    }
}
