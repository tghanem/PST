using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.impl;
using pst.impl.messaging.changetracking;
using pst.impl.ndb;
using pst.interfaces.messaging.changetracking.model;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var trackedObjects = new Dictionary<ObjectPath, NodeTrackingObject>();

            var trackedRecipientTables = new Dictionary<ObjectPath, RecipientTableTrackingObject>();

            var cachedHeaderHolder = new DefaultDataHolder<Header>();

            var cachedBTPages = new DictionaryBasedCache<BID, BTPage>();

            var cachedInternalDataBlocks = new DictionaryBasedCache<BID, InternalDataBlock>();

            return
                new PSTFile(
                    new ObjectTracker(trackedObjects),
                    new RecipientTracker(trackedRecipientTables),
                    CreateHeaderBasedStringEncoder(stream, cachedHeaderHolder),
                    CreateNodeEntryFinder(stream, cachedBTPages, cachedHeaderHolder), 
                    CreateRowIndexReader(stream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreatePropertyIdToNameMap(stream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreatePropertyContextBasedPropertyReader(stream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    CreateTagBasedTableContextBasedPropertyReader(stream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder), 
                    new HeaderBasedNIDAllocator(CreateHeaderUsageProvider(stream, cachedHeaderHolder)), 
                    new ChangesApplier(trackedObjects, null));
        }
    }
}
