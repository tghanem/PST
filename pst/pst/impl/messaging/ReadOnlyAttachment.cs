using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

namespace pst.impl.messaging
{
    class ReadOnlyAttachment : IReadOnlyAttachment
    {
        private readonly IDecoder<NID> nidDecoder;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IPropertyContextBasedComponent readOnlyComponent;

        public ReadOnlyAttachment(
            IDecoder<NID> nidDecoder,
            INodeEntryFinder nodeEntryFinder,
            IPropertyContextBasedComponent readOnlyComponent)
        {
            this.nidDecoder = nidDecoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.readOnlyComponent = readOnlyComponent;
        }

        public Maybe<NID> GetEmbeddedMessageNodeId(NodePath attachmentNodePath)
        {
            var entry = nodeEntryFinder.GetEntry(attachmentNodePath);

            if (entry.HasNoValue)
            {
                return Maybe<NID>.NoValue();
            }

            var attachMethodPropertyValue =
                readOnlyComponent.GetProperty(
                    new TaggedPropertyPath(attachmentNodePath, MAPIProperties.PidTagAttachMethod));

            if (attachMethodPropertyValue.HasNoValue ||
               !attachMethodPropertyValue.Value.Value.HasFlag(MAPIProperties.afEmbeddedMessage))
            {
                return Maybe<NID>.NoValue();
            }

            var attachDataObject =
                readOnlyComponent.GetProperty(
                    new TaggedPropertyPath(attachmentNodePath, MAPIProperties.PidTagAttachDataObject));

            return nidDecoder.Decode(attachDataObject.Value.Value.Take(4));
        }
    }
}
