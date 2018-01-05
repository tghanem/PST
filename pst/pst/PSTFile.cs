using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging.model;
using pst.interfaces.messaging.model.changetracking;
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
        private readonly ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader;

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
            ITableContextBasedPropertyReader<Tag> tableContextBasedPropertyReader)
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
        }

        public MessageStore MessageStore => new MessageStore(changesTracker, propertyNameToIdMap, propertyContextBasedPropertyReader);

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId =
                MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId =
                entryIdDecoder.Decode(ipmSubtreeEntryId.Value.Value);

            return
                new Folder(
                    NodePath.OfValue(AllocatedNodeId.OfValue(entryId.NID)),
                    changesTracker,
                    stringEncoder,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    nidDecoder,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextReader,
                    tableContextBasedPropertyReader);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
