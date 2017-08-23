using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.messaging;
using System.Linq;

namespace pst
{
    public class Attachment
    {
        private readonly NID[] attachmentNodePath;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedReadOnlyComponent readOnlyComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Attachment(
            NID[] attachmentNodePath,
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
                    attachmentNodePath.Concat(new[] { embeddedMessageNID.Value }).ToArray(),
                    readOnlyMessage,
                    readOnlyAttachment,
                    readOnlyComponent, readOnlyComponentForRecipient);
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(attachmentNodePath, propertyTag);
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(attachmentNodePath, propertyTag);
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return readOnlyComponent.GetProperty(attachmentNodePath, propertyTag);
        }
    }
}
