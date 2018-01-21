using pst.encodables.ndb;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Message : ObjectBase
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

        private bool preExistingRecipientsLoaded;
        private bool preExistingAttachmentsLoaded;

        internal Message(
            ObjectPath objectPath,
            IObjectTracker objectTracker,
            IRecipientTracker recipientTracker,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader rowIndexReader,
            ITableContextReader tableContextReader,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader) : base(objectPath, objectTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.objectPath = objectPath;
            this.objectTracker = objectTracker;
            this.recipientTracker = recipientTracker;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
        }

        public Recipient[] GetRecipients()
        {
            if (!preExistingRecipientsLoaded)
            {
                LoadPreExistingRecipients();
                preExistingRecipientsLoaded = true;
            }

            var recipientTableNodeId = recipientTracker.GetTrackedRecipientTable(objectPath);

            return
                recipientTracker
                .GetTrackedRecipients(
                    messageObjectPath: objectPath,
                    recipientTableNodeId: recipientTableNodeId,
                    recipientStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    recipientRowId =>
                    new Recipient(
                        objectPath,
                        recipientTableNodeId,
                        recipientRowId,
                        recipientTracker,
                        propertyNameToIdMap,
                        tableContextBasedPropertyReader))
                .ToArray();
        }

        public Attachment[] GetAttachments()
        {
            if (!preExistingAttachmentsLoaded)
            {
                LoadPreExistingAttachments();
                preExistingAttachmentsLoaded = true;
            }

            return
                objectTracker
                .GetChildObjects(
                    objectPath: objectPath,
                    childType: ObjectTypes.Attachment,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    path =>
                    new Attachment(
                        path,
                        objectTracker,
                        recipientTracker,
                        propertyNameToIdMap,
                        propertyContextBasedPropertyReader,
                        nodeEntryFinder,
                        rowIndexReader,
                        tableContextReader,
                        tableContextBasedPropertyReader))
                .ToArray();
        }

        private void LoadPreExistingAttachments()
        {
            var entry = nodeEntryFinder.GetEntry(new[] { objectPath.LocalNodeId });

            if (entry.HasNoValue)
            {
                return;
            }

            var tableContextNodeId = entry.Value.GetChildOfType(Constants.NID_TYPE_ATTACHMENT_TABLE).LocalSubnodeId;

            foreach (var rowId in rowIndexReader.GetAllRowIds(new[] { objectPath.LocalNodeId, tableContextNodeId }))
            {
                objectTracker.TrackObject(objectPath.Add(NID.OfValue(rowId.RowId)), ObjectTypes.Attachment, ObjectStates.Loaded);
            }
        }

        private void LoadPreExistingRecipients()
        {
            var entry = nodeEntryFinder.GetEntry(new[] { objectPath.LocalNodeId });

            if (entry.HasNoValue)
            {
                return;
            }

            var tableContextNodeId = entry.Value.GetChildOfType(Constants.NID_TYPE_RECIPIENT_TABLE).LocalSubnodeId;

            recipientTracker.TrackRecipientTable(objectPath, tableContextNodeId, ObjectStates.Loaded);

            foreach (var rowId in rowIndexReader.GetAllRowIds(new[] { objectPath.LocalNodeId, tableContextNodeId }))
            {
                recipientTracker.TrackRecipient(objectPath, tableContextNodeId, rowId.RowId.ToInt32(), ObjectStates.Loaded);
            }
        }
    }
}
