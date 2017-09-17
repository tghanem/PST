using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst.impl.messaging
{
    class ReadOnlyMessage : IReadOnlyMessage
    {
        private readonly ITableContextReader tableContextReader;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly IDecoder<NID> nidDecoder;

        private readonly INodeEntryFinder nodeEntryFinder;

        public ReadOnlyMessage(
            ITableContextReader tableContextReader,
            IRowIndexReader<NID> rowIndexReader,
            IDecoder<NID> nidDecoder,
            INodeEntryFinder nodeEntryFinder)
        {
            this.tableContextReader = tableContextReader;
            this.rowIndexReader = rowIndexReader;
            this.nidDecoder = nidDecoder;
            this.nodeEntryFinder = nodeEntryFinder;
        }

        public Maybe<NID> GetRecipientTableNodeId(NodePath messageNodePath)
        {
            var entry = nodeEntryFinder.GetEntry(messageNodePath);

            if (entry.HasNoValue)
            {
                return Maybe<NID>.NoValue();
            }

            var recipientTableEntry =
                entry.Value.ChildNodes.First(s => s.LocalSubnodeId.Type == Globals.NID_TYPE_RECIPIENT_TABLE);

            return recipientTableEntry.LocalSubnodeId;
        }

        public Maybe<Tag[]> GetTagsForRecipients(NodePath messageNodePath)
        {
            var entry = nodeEntryFinder.GetEntry(messageNodePath);

            if (entry.HasNoValue)
            {
                return Maybe<Tag[]>.NoValue();
            }

            var recipientTableEntry =
                entry.Value.ChildNodes.First(s => s.LocalSubnodeId.Type == Globals.NID_TYPE_RECIPIENT_TABLE);

            return
                tableContextReader
                .GetAllRows(messageNodePath.Add(recipientTableEntry.LocalSubnodeId))
                .Select(rowId => Tag.OfValue(rowId.RowId))
                .ToArray();
        }

        public Maybe<NID[]> GetNodeIdsForAttachments(NodePath messageNodePath)
        {
            var entry = nodeEntryFinder.GetEntry(messageNodePath);

            if (entry.HasNoValue)
            {
                return Maybe<NID[]>.NoValue();
            }

            var attachmentsTableEntry =
                entry.Value.ChildNodes.First(s => s.LocalSubnodeId.Type == Globals.NID_TYPE_ATTACHMENT_TABLE);

            return
                rowIndexReader
                .GetAllRowIds(messageNodePath.Add(attachmentsTableEntry.LocalSubnodeId))
                .Select(rowId => nidDecoder.Decode(rowId.RowId))
                .ToArray();
        }
    }
}
