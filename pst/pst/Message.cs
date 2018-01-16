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
        private readonly NodePath nodePath;
        private readonly IChangesTracker changesTracker;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;

        private bool preExistingRecipientsLoaded;
        private bool preExistingAttachmentsLoaded;

        internal Message(
            NodePath nodePath,
            IChangesTracker changesTracker,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader rowIndexReader,
            ITableContextReader tableContextReader,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader) : base(nodePath, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.changesTracker = changesTracker;
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

            var recipientTableNodePath =
                changesTracker
                .GetChildren(
                    parentNodePath: nodePath,
                    childType: ObjectTypes.RecipientTable,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .First();

            return
                changesTracker
                .GetAssociatedObjects(recipientTableNodePath)
                .Select(path => new Recipient(path, changesTracker, propertyNameToIdMap, tableContextBasedPropertyReader))
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
                changesTracker
                .GetChildren(
                    parentNodePath: nodePath,
                    childType: ObjectTypes.Attachment,
                    childStatePredicate: s => s == ObjectStates.New || s == ObjectStates.Loaded)
                .Select(
                    path =>
                    new Attachment(
                        path,
                        changesTracker,
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
            var entry = nodeEntryFinder.GetEntry(nodePath.AllocatedIds);

            if (entry.HasNoValue)
            {
                return;
            }

            var tableContextNodeId = entry.Value.GetChildOfType(Constants.NID_TYPE_ATTACHMENT_TABLE).LocalSubnodeId;

            var tableContextNodePath = nodePath.Add(AllocatedNodeId.OfValue(tableContextNodeId));

            foreach (var tableRow in tableContextReader.GetAllRows(tableContextNodePath))
            {
                var attachmentNodeId = AllocatedNodeId.OfValue(NID.OfValue(tableRow.RowId.RowId));

                changesTracker.TrackNode(
                    nodePath.Add(attachmentNodeId),
                    ObjectTypes.Attachment,
                    ObjectStates.Loaded,
                    nodePath);
            }
        }

        private void LoadPreExistingRecipients()
        {
            var entry = nodeEntryFinder.GetEntry(nodePath.AllocatedIds);

            if (entry.HasNoValue)
            {
                return;
            }

            var tableContextNodeId = entry.Value.GetChildOfType(Constants.NID_TYPE_RECIPIENT_TABLE).LocalSubnodeId;

            var tableContextNodePath = nodePath.Add(AllocatedNodeId.OfValue(tableContextNodeId));

            changesTracker.TrackNode(tableContextNodePath, ObjectTypes.RecipientTable, ObjectStates.Loaded, nodePath);

            foreach (var rowId in tableContextReader.GetAllRows(tableContextNodePath))
            {
                changesTracker.Associate(
                    tableContextNodePath,
                    rowId.RowId,
                    ObjectTypes.Recipient,
                    ObjectStates.Loaded);
            }
        }
    }
}
