using pst.encodables;
using pst.interfaces;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.interfaces.ndb;

namespace pst
{
    public partial class PSTFile
    {
        private readonly IObjectTracker objectTracker;
        private readonly IRecipientTracker recipientTracker;
        private readonly IEncoder<string> stringEncoder;
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IRowIndexReader rowIndexReader;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;
        private readonly ITableContextBasedPropertyReader tableContextBasedPropertyReader;
        private readonly INIDAllocator nodeIdAllocator;
        private readonly IChangesApplier changesApplier;

        private PSTFile(
            IObjectTracker objectTracker,
            IRecipientTracker recipientTracker,
            IEncoder<string> stringEncoder,
            INodeEntryFinder nodeEntryFinder,
            IRowIndexReader rowIndexReader,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader,
            ITableContextBasedPropertyReader tableContextBasedPropertyReader,
            INIDAllocator nodeIdAllocator,
            IChangesApplier changesApplier)
        {
            this.objectTracker = objectTracker;
            this.recipientTracker = recipientTracker;
            this.stringEncoder = stringEncoder;
            this.nodeEntryFinder = nodeEntryFinder;
            this.rowIndexReader = rowIndexReader;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
            this.tableContextBasedPropertyReader = tableContextBasedPropertyReader;
            this.nodeIdAllocator = nodeIdAllocator;
            this.changesApplier = changesApplier;
        }

        public MessageStore MessageStore
        {
            get
            {
                if (!objectTracker.IsObjectTracked(MessageStore.StorePath))
                {
                    objectTracker.TrackObject(MessageStore.StorePath, ObjectTypes.Store, ObjectStates.Loaded);
                }

                return new MessageStore(objectTracker, propertyNameToIdMap, propertyContextBasedPropertyReader);
            }
        }

        public Folder GetRootMailboxFolder()
        {
            var ipmSubtreeEntryId = MessageStore.GetProperty(MAPIProperties.PidTagIpmSubTreeEntryId);

            var entryId = EntryId.OfValue(ipmSubtreeEntryId.Value.Value);

            var nodePath = new ObjectPath(new[] { entryId.NID });

            if (!objectTracker.IsObjectTracked(nodePath))
            {
                objectTracker.TrackObject(nodePath, ObjectTypes.Folder, ObjectStates.Loaded);
            }

            return
                new Folder(
                    nodePath,
                    objectTracker,
                    recipientTracker, 
                    stringEncoder,
                    propertyNameToIdMap,
                    propertyContextBasedPropertyReader,
                    nodeEntryFinder,
                    rowIndexReader,
                    tableContextBasedPropertyReader,
                    nodeIdAllocator);
        }

        public void Save()
        {
            changesApplier.Apply();
        }
    }
}
