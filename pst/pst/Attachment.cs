using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.ndb;
using pst.utilities;
using System.Linq;
using pst.interfaces.ltp;

namespace pst
{
    public class Attachment
    {
        private readonly BID attachmentDataBlockId;
        private readonly BID attachmentSubnodeBlockId;
        private readonly IDecoder<NID> nidDecoder;
        private readonly ITCReader<NID> nidBasedTableContextReader;
        private readonly ITCReader<Tag> tagBasedTableContextReader;
        private readonly ISubNodesEnumerator subnodesEnumerator;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPCBasedPropertyReader pcBasedPropertyReader;

        internal Attachment(
            BID attachmentDataBlockId,
            BID attachmentSubnodeBlockId,
            IDecoder<NID> nidDecoder,
            ITCReader<NID> nidBasedTableContextReader,
            ITCReader<Tag> tagBasedTableContextReader,
            ISubNodesEnumerator subnodesEnumerator,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPCBasedPropertyReader pcBasedPropertyReader)
        {
            this.attachmentDataBlockId = attachmentDataBlockId;
            this.attachmentSubnodeBlockId = attachmentSubnodeBlockId;

            this.nidDecoder = nidDecoder;
            this.nidBasedTableContextReader = nidBasedTableContextReader;
            this.tagBasedTableContextReader = tagBasedTableContextReader;

            this.subnodesEnumerator = subnodesEnumerator;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.pcBasedPropertyReader = pcBasedPropertyReader;
        }

        public Maybe<Message> GetEmbeddedMessage()
        {
            var attachMethodPropertyValue =
                pcBasedPropertyReader.ReadProperty(
                    attachmentDataBlockId,
                    attachmentSubnodeBlockId,
                    MAPIProperties.PidTagAttachMethod);

            if (attachMethodPropertyValue.HasValue &&
                attachMethodPropertyValue.Value.Value.HasFlag(MAPIProperties.afEmbeddedMessage))
            {
                var attachDataObject =
                    pcBasedPropertyReader.ReadProperty(
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
                        nidBasedTableContextReader,
                        tagBasedTableContextReader,
                        subnodesEnumerator,
                        propertyNameToIdMap, 
                        pcBasedPropertyReader);
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
                pcBasedPropertyReader.ReadProperty(
                    attachmentDataBlockId,
                    attachmentSubnodeBlockId,
                    propertyTag);
        }
    }
}
