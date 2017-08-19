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
    public class Attachment
    {
        private readonly BID attachmentDataBlockId;
        private readonly BID attachmentSubnodeBlockId;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyReader propertyReader;

        internal Attachment(
            BID attachmentDataBlockId,
            BID attachmentSubnodeBlockId,
            IDecoder<NID> nidDecoder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            ITableContextBasedPropertyReader<NID> nidBasedTableContextBasedPropertyReader,
            ITableContextBasedPropertyReader<Tag> tagBasedTableContextBasedPropertyReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyReader propertyReader)
        {
            this.attachmentDataBlockId = attachmentDataBlockId;
            this.attachmentSubnodeBlockId = attachmentSubnodeBlockId;

            this.nidDecoder = nidDecoder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.nidBasedTableContextBasedPropertyReader = nidBasedTableContextBasedPropertyReader;
            this.tagBasedTableContextBasedPropertyReader = tagBasedTableContextBasedPropertyReader;

            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyReader = propertyReader;
        }

        public Maybe<Message> GetEmbeddedMessage()
        {
            var attachMethodPropertyValue =
                propertyReader.ReadProperty(
                    attachmentDataBlockId,
                    attachmentSubnodeBlockId,
                    MAPIProperties.PidTagAttachMethod);

            if (attachMethodPropertyValue.HasValue &&
                attachMethodPropertyValue.Value.Value.HasFlag(MAPIProperties.afEmbeddedMessage))
            {
                var attachDataObject =
                    propertyReader.ReadProperty(
                        attachmentDataBlockId,
                        attachmentSubnodeBlockId,
                        MAPIProperties.PidTagAttachDataObject);

                var parser =
                    BinaryDataParser.OfValue(attachDataObject.Value.Value);

                var nid =
                    parser.TakeAndSkip(4, nidDecoder);

                var subnodes =
                    subnodesEnumerator.Enumerate(attachmentSubnodeBlockId);

                var embeddedMessageNodeEntry =
                    subnodes.First(s => s.LocalSubnodeId.Value == nid.Value);

                return
                    new Message(
                        embeddedMessageNodeEntry.DataBlockId,
                        embeddedMessageNodeEntry.SubnodeBlockId,
                        nidDecoder,
                        rowIndexReader,
                        tableContextReader, 
                        nidBasedTableContextBasedPropertyReader,
                        tagBasedTableContextBasedPropertyReader,
                        subnodesEnumerator,
                        propertyNameToIdMap, 
                        propertyReader);
            }

            return Maybe<Message>.NoValue();
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
            return
                propertyReader.ReadProperty(
                    attachmentDataBlockId,
                    attachmentSubnodeBlockId,
                    propertyTag);
        }
    }
}
