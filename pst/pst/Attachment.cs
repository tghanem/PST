using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
using pst.interfaces.ndb;

namespace pst
{
    public class Attachment : ObjectBase
    {
        private readonly NodePath nodePath;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IChangesTracker changesTracker;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;

        internal Attachment(
            NodePath nodePath,
            IChangesTracker changesTracker,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            IDecoder<NID> nidDecoder,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader) : base(nodePath, changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader)
        {
            this.nodePath = nodePath;
            this.nidDecoder = nidDecoder;
            this.changesTracker = changesTracker;
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

            var embeddedMessageNodePath = nodePath.Add(embeddedMessageNodeId.Value);

            changesTracker.TrackNode(
                embeddedMessageNodePath,
                ObjectTypes.Message,
                ObjectStates.Loaded,
                nodePath);

            return
                new Message(
                    embeddedMessageNodePath,
                    nidDecoder,
                    changesTracker,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextReader,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    tableContextBasedPropertyReader);
        }

        private Maybe<NodeId> GetEmbeddedMessageNodeId()
        {
            var entry = nodeEntryFinder.GetEntry(nodePath.AllocatedIds);

            if (entry.HasNoValue)
            {
                return Maybe<NodeId>.NoValue();
            }

            var attachMethodPropertyValue = GetProperty(MAPIProperties.PidTagAttachMethod);

            if (!attachMethodPropertyValue.HasValueAnd(v => v.Value.HasFlag(MAPIProperties.afEmbeddedMessage)))
            {
                return Maybe<NodeId>.NoValue();
            }

            var attachDataObject = GetProperty(MAPIProperties.PidTagAttachDataObject);

            return AllocatedNodeId.OfValue(nidDecoder.Decode(attachDataObject.Value.Value.Take(4)));
        }
    }
}
