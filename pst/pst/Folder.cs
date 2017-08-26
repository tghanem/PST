using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces.messaging;
using System.Linq;

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IReadOnlyFolder readOnlyFolder;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedReadOnlyComponent readOnlyComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Folder(
            NID nodeId,
            IReadOnlyFolder readOnlyFolder,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedReadOnlyComponent readOnlyComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.nodeId = nodeId;
            this.readOnlyFolder = readOnlyFolder;
            this.readOnlyMessage = readOnlyMessage;
            this.readOnlyAttachment = readOnlyAttachment;
            this.readOnlyComponent = readOnlyComponent;
            this.readOnlyComponentForRecipient = readOnlyComponentForRecipient;
        }

        public Folder[] GetSubFolders()
        {
            var nodeIds = readOnlyFolder.GetNodeIdsForSubFolders(nodeId);

            if (nodeIds.HasNoValue)
            {
                return new Folder[0];
            }

            return
                nodeIds
                .Value
                .Select(
                    nid =>
                    new Folder(
                        nid,
                        readOnlyFolder,
                        readOnlyMessage,
                        readOnlyAttachment,
                        readOnlyComponent,
                        readOnlyComponentForRecipient))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var nodeIds = readOnlyFolder.GetNodeIdsForContents(nodeId);

            if (nodeIds.HasNoValue)
            {
                return new Message[0];
            }

            return
                nodeIds
                .Value
                .Select(
                    nid =>
                    new Message(
                        NodePath.OfValue(nid),
                        readOnlyMessage,
                        readOnlyAttachment,
                        readOnlyComponent,
                        readOnlyComponentForRecipient))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new NumericalTaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new StringTaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                readOnlyComponent.GetProperty(
                    new TaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag));
        }
    }
}
