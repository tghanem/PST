using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;

namespace pst
{
    public class Message
    {
        private readonly BID nodeBlockId;
        private readonly BID subnodeBlockId;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader<NID> tableContextReader;
        private readonly ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyReader propertyReader;

        internal Message(
            BID nodeBlockId,
            BID subnodeBlockId,
            IDecoder<NID> nidDecoder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader<NID> tableContextReader,
            ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader,
            ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyReader propertyReader)
        {
            this.nodeBlockId = nodeBlockId;
            this.subnodeBlockId = subnodeBlockId;

            this.nidDecoder = nidDecoder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyReader = propertyReader;
            this.nidBasedTableContextBasedPropertyReader = nidBasedTableContextBasedPropertyReader;
            this.tagBasedTableContextBasedPropertyReader = tagBasedTableContextBasedPropertyReader;
        }

        public Recipient[] GetRecipients()
        {
            var subnodes =
                subnodesEnumerator.Enumerate(subnodeBlockId);

            var recipientTableEntry =
                subnodes.First(s => s.LocalSubnodeId.Type == Globals.NID_TYPE_RECIPIENT_TABLE);

            var rows =
                tableContextReader.GetAllRows(recipientTableEntry.DataBlockId, recipientTableEntry.SubnodeBlockId);

            return
                rows
                .Select(
                    r =>
                    {
                        return
                            new Recipient(
                                recipientTableEntry.DataBlockId,
                                recipientTableEntry.SubnodeBlockId,
                                Tag.OfValue(r.RowId),
                                propertyNameToIdMap,
                                tagBasedTableContextBasedPropertyReader);
                    })
                .ToArray();
        }

        public Attachment[] GetAttachments()
        {
            var subnodes =
                subnodesEnumerator.Enumerate(subnodeBlockId);

            var attachmentsTableEntry =
                subnodes.First(s => s.LocalSubnodeId.Type == Globals.NID_TYPE_ATTACHMENT_TABLE);

            var rowsIds =
                rowIndexReader.GetAllRowIds(attachmentsTableEntry.DataBlockId);

            return
                rowsIds
                .Select(
                    id =>
                    {
                        var attachmentNodeId = nidDecoder.Decode(id.RowId);

                        var attachmentSubnodeEntry =
                            subnodes.First(s => s.LocalSubnodeId.Value == attachmentNodeId.Value);

                        return
                            new Attachment(
                                attachmentSubnodeEntry.DataBlockId,
                                attachmentSubnodeEntry.SubnodeBlockId,
                                nidDecoder,
                                rowIndexReader,
                                tableContextReader, 
                                nidBasedTableContextBasedPropertyReader,
                                tagBasedTableContextBasedPropertyReader,
                                subnodesEnumerator,
                                propertyNameToIdMap,
                                propertyReader);
                    })
                .ToArray();
        }

        public Maybe<PropertyValue> GetProperty(NumericalPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(StringPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return GetProperty(new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(PropertyTag propertyTag)
        {
            return propertyReader.ReadProperty(nodeBlockId, subnodeBlockId, propertyTag);
        }
    }
}
