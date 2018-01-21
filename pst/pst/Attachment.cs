using pst.core;
using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.interfaces.ndb;

namespace pst
{
    public class Attachment : ObjectBase
    {
        private readonly ObjectPath objectPath;
        private readonly IObjectTracker objectTracker;
        private readonly IRecipientTracker recipientTracker;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;

        internal Attachment(
            ObjectPath objectPath,
            IObjectTracker objectTracker,
            IRecipientTracker recipientTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader rowIndexReader,
            ITableContextReader tableContextReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader) : base(objectPath, objectTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.objectPath = objectPath;
            this.objectTracker = objectTracker;
            this.recipientTracker = recipientTracker;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Maybe<Message> GetEmbeddedMessage()
        {
            var embeddedMessageNodeId = GetEmbeddedMessageNodeId();

            if (embeddedMessageNodeId.HasNoValue)
            {
                return Maybe<Message>.NoValue();
            }

            var embeddedMessageNodePath = objectPath.Add(embeddedMessageNodeId.Value);

            objectTracker.TrackObject(
                embeddedMessageNodePath,
                ObjectTypes.Message,
                ObjectStates.Loaded);

            return
                new Message(
                    embeddedMessageNodePath,
                    objectTracker,
                    recipientTracker,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextReader,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    tableContextBasedPropertyReader);
        }

        private Maybe<NID> GetEmbeddedMessageNodeId()
        {
            var entry = nodeEntryFinder.GetEntry(new[] { objectPath.ParentObjectPath.LocalNodeId, objectPath.LocalNodeId });

            if (entry.HasNoValue)
            {
                return Maybe<NID>.NoValue();
            }

            var attachMethodPropertyValue = GetProperty(MAPIProperties.PidTagAttachMethod);

            if (!attachMethodPropertyValue.HasValueAnd(v => v.Value.HasFlag(MAPIProperties.afEmbeddedMessage)))
            {
                return Maybe<NID>.NoValue();
            }

            var attachDataObject = GetProperty(MAPIProperties.PidTagAttachDataObject);

            return NID.OfValue(attachDataObject.Value.Value.Take(4));
        }
    }
}
