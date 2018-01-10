using pst.core;
using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.interfaces.ndb;
using System;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IDecoder<EntryId> entryIdDecoder;
        private readonly IDecoder<NID> nidDecoder;
        private readonly IChangesTracker changesTracker;
        private readonly IEncoder<string> stringEncoder;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader<NID> rowIndexReader;
        private readonly ITableContextReader tableContextReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;
        private readonly IUnallocatedNodeIdGenerator nodeIdGenerator;

        private PSTFile(
            IDecoder<EntryId> entryIdDecoder,
            IDecoder<NID> nidDecoder,
            IChangesTracker changesTracker,
            IEncoder<string> stringEncoder,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader<NID> rowIndexReader,
            ITableContextReader tableContextReader,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader,
            IUnallocatedNodeIdGenerator nodeIdGenerator)
        {
            this.entryIdDecoder = entryIdDecoder;
            this.nidDecoder = nidDecoder;
            this.changesTracker = changesTracker;
            this.stringEncoder = stringEncoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.tableContextReader = tableContextReader;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
            this.nodeIdGenerator = nodeIdGenerator;
        }

        public MessageStore MessageStore
        {
            get
            {
                if (!changesTracker.IsObjectTracked(MessageStore.StorePath))
                {
                    changesTracker.TrackNode(
                        MessageStore.StorePath,
                        ObjectTypes.Store,
                        ObjectStates.Loaded,
                        Maybe<NodePath>.NoValue());
                }

                return new MessageStore(changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader);
            }
        }

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId = MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId = entryIdDecoder.Decode(ipmSubtreeEntryId.Value.Value);

            var nodePath = NodePath.OfValue(AllocatedNodeId.OfValue(entryId.NID));

            if (!changesTracker.IsObjectTracked(nodePath))
            {
                changesTracker.TrackNode(
                    nodePath,
                    ObjectTypes.Folder,
                    ObjectStates.Loaded,
                    Maybe<NodePath>.NoValue());
            }

            return
                new Folder(
                    nodePath,
                    changesTracker,
                    stringEncoder,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    nidDecoder,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextReader,
                    tableContextBasedPropertyReader,
                    nodeIdGenerator);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
