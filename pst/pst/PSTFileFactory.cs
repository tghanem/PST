using pst.encodables.ndb;
using pst.impl;
using pst.impl.messaging.changetracking;
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

            return
                new PSTFile(
                    new ObjectTracker(trackedObjects),
                    new RecipientTracker(trackedRecipientTables),
                    CreateHeaderBasedStringEncoder(stream),
                    CreateNodeEntryFinder(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreateRowIndexReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreateTableContextReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreatePropertyIdToNameMap(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreatePropertyContextBasedPropertyReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreateTagBasedTableContextBasedPropertyReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    null,
                    new ChangesApplier(trackedObjects));
        }
    }
}
