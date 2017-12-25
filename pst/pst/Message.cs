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
        private readonly IPropertyContextBasedComponent propertyContextBasedComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Message(
            NodePath messageNodePath,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedComponent propertyContextBasedComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.messageNodePath = messageNodePath;
            this.readOnlyMessage = readOnlyMessage;
            this.propertyContextBasedComponent = propertyContextBasedComponent;
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
                        propertyContextBasedComponent, readOnlyComponentForRecipient))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new NumericalTaggedPropertyPath(messageNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new StringTaggedPropertyPath(messageNodePath, propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new TaggedPropertyPath(messageNodePath, propertyTag));
        }
    }
}
