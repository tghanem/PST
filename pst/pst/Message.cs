using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Message : ObjectBase
    {
        private readonly NodePath nodePath;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IChangesTracker changesTracker;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader;

        private bool preExistingRecipientsLoaded;
        private bool preExistingAttachmentsLoaded;

        internal Message(
            NodePath nodePath,
            IDecoder<NID> nidDecoder,
            IChangesTracker changesTracker,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader) : base(nodePath, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.nidDecoder = nidDecoder;
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
                        nidDecoder,
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

            foreach (var rowId in tableContextReader.GetAllRows(tableContextNodePath))
            {
                var attachmentNodeId = AllocatedNodeId.OfValue(nidDecoder.Decode(rowId.RowId));

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
                    Tag.OfValue(rowId.RowId),
                    ObjectTypes.Recipient,
                    ObjectStates.Loaded);
            }
        }
    }
}
