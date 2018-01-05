using pst.core;
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

        internal Message(
            NodePath nodePath,
            IDecoder<NID> nidDecoder,
            IChangesTracker changesTracker,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader) : base(nodePath, ObjectTypes.Message, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
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
            var entry = nodeEntryFinder.GetEntry(nodePath.AllocatedIds);

            if (entry.HasNoValue)
            {
                return new Recipient[0];
            }

            var recipientNodeId = entry.Value.GetChildOfType(Constants.NID_TYPE_RECIPIENT_TABLE).LocalSubnodeId;

            return
                tableContextReader
                .GetAllRows(nodePath.Add(AllocatedNodeId.OfValue(recipientNodeId)))
                .Select(
                    rowId =>
                    new Recipient(
                        nodePath.Add(AllocatedNodeId.OfValue(recipientNodeId)),
                        Tag.OfValue(rowId.RowId),
                        changesTracker,
                        propertyNameToIdMap,
                        tableContextBasedPropertyReader))
                .ToArray();
        }

        public Attachment[] GetAttachments()
        {
            var nodeIdsForAttachments = GetNodeIdsForAttachments();

            if (nodeIdsForAttachments.HasNoValue)
            {
                return new Attachment[0];
            }

            return
                nodeIdsForAttachments
                .Value
                .Select(
                    nodeId =>
                    new Attachment(
                        nodePath.Add(nodeId),
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

        private Maybe<NodeId[]> GetNodeIdsForAttachments()
        {
            var entry = nodeEntryFinder.GetEntry(nodePath.AllocatedIds);

            if (entry.HasNoValue)
            {
                return Maybe<NodeId[]>.NoValue();
            }

            var attachmentNodeId = entry.Value.GetChildOfType(Constants.NID_TYPE_ATTACHMENT_TABLE).LocalSubnodeId;

            return
                rowIndexReader
                .GetAllRowIds(nodePath.Add(AllocatedNodeId.OfValue(attachmentNodeId)).AllocatedIds)
                .Select(rowId => AllocatedNodeId.OfValue(nidDecoder.Decode(rowId.RowId)))
                .ToArray();
        }
    }
}
