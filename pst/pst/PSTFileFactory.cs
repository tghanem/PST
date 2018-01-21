using pst.encodables.ndb;
using pst.impl;
using pst.impl.messaging.changetracking;
using pst.impl.ndb;
using pst.interfaces.messaging.changetracking;
using pst.interfaces.model;
using pst.interfaces.ndb;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var cachedNodeEntries = new DictionaryBasedCache<NID[], NodeEntry>();

            var dataBlockEntryCache = new DictionaryBasedCache<BID, DataBlockEntry>();

            var trackedObjects = new Dictionary<ObjectPath, NodeTrackingObject>();

            var trackedRecipientTables = new Dictionary<ObjectPath, RecipientTableTrackingObject>();

            var cachedHeaderHolder = new DefaultDataHolder<Header>();

            return
                new PSTFile(
                    new ObjectTracker(trackedObjects),
                    new RecipientTracker(trackedRecipientTables),
                    CreateHeaderBasedStringEncoder(stream, cachedHeaderHolder),
                    CreateNodeEntryFinder(stream, cachedNodeEntries, dataBlockEntryCache, cachedHeaderHolder),
                    CreateRowIndexReader(stream, cachedNodeEntries, dataBlockEntryCache, cachedHeaderHolder),
                    CreatePropertyIdToNameMap(stream, cachedNodeEntries, dataBlockEntryCache, cachedHeaderHolder),
                    CreatePropertyContextBasedPropertyReader(stream, cachedNodeEntries, dataBlockEntryCache, cachedHeaderHolder),
                    CreateTagBasedTableContextBasedPropertyReader(stream, cachedNodeEntries, dataBlockEntryCache, cachedHeaderHolder),
                    new HeaderBasedNIDAllocator(CreateHeaderUsageProvider(stream, cachedHeaderHolder)), 
                    new ChangesApplier(trackedObjects, null));
        }
    }
}
