using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using System;
using System.Linq;

namespace pst
{
    public class Folder
    {
        private readonly NID nodeId;
        private readonly IFolder folder;
        private readonly IEncoder<string> stringEncoder;
        private readonly IReadOnlyMessage readOnlyMessage;
        private readonly IReadOnlyAttachment readOnlyAttachment;
        private readonly IPropertyContextBasedComponent propertyContextBasedComponent;
        private readonly ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient;

        internal Folder(
            NID nodeId,
            IFolder folder,
            IEncoder<string> stringEncoder,
            IReadOnlyMessage readOnlyMessage,
            IReadOnlyAttachment readOnlyAttachment,
            IPropertyContextBasedComponent propertyContextBasedComponent,
            ITableContextBasedReadOnlyComponent<Tag> readOnlyComponentForRecipient)
        {
            this.nodeId = nodeId;
            this.folder = folder;
            this.stringEncoder = stringEncoder;
            this.readOnlyMessage = readOnlyMessage;
            this.readOnlyAttachment = readOnlyAttachment;
            this.propertyContextBasedComponent = propertyContextBasedComponent;
            this.readOnlyComponentForRecipient = readOnlyComponentForRecipient;
        }

        public Folder NewFolder(string displayName)
        {
            var newFolderNodeId = folder.NewFolderNodeId();

            propertyContextBasedComponent.SetProperty(
                new TaggedPropertyPath(
                    NodePath.OfValue(newFolderNodeId),
                    MAPIProperties.PidTagDisplayName),
                new PropertyValue(stringEncoder.Encode(displayName)));

            return
                new Folder(
                    newFolderNodeId,
                    folder,
                    stringEncoder,
                    readOnlyMessage,
                    readOnlyAttachment,
                    propertyContextBasedComponent,
                    readOnlyComponentForRecipient);
        }

        public Folder[] GetSubFolders()
        {
            var nodeIds = folder.GetNodeIdsForSubFolders(nodeId);

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
                        folder,
                        stringEncoder, 
                        readOnlyMessage,
                        readOnlyAttachment,
                        propertyContextBasedComponent,
                        readOnlyComponentForRecipient))
                .ToArray();
        }

        public Message[] GetMessages()
        {
            var nodeIds = folder.GetNodeIdsForContents(nodeId);

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
                        propertyContextBasedComponent,
                        readOnlyComponentForRecipient))
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new NumericalTaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new StringTaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return
                propertyContextBasedComponent.GetProperty(
                    new TaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag));
        }

        public void SetProperty(NumericalPropertyTag propertyTag, PropertyValue propertValue)
        {
            propertyContextBasedComponent.SetProperty(
                new NumericalTaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag), propertValue);
        }

        public void SetProperty(StringPropertyTag propertyTag, PropertyValue propertyValue)
        {
            propertyContextBasedComponent.SetProperty(
                new StringTaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag), propertyValue);
        }

        public void SetProperty(PropertyTag propertyTag, PropertyValue propertyValue)
        {
            propertyContextBasedComponent.SetProperty(
                new TaggedPropertyPath(NodePath.OfValue(nodeId), propertyTag), propertyValue);
        }
    }
}
