using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders;
using pst.impl.decoders.ndb;
using pst.impl.messaging;
using pst.impl.messaging.changetracking;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var cachedNodeEntries = new DictionaryBasedCache<NID[], NodeEntry>();

            var dataBlockEntryCache = new DictionaryBasedCache<BID, DataBlockEntry>();

            var unallocatedNodeIdGenerator = new UnallocatedNodeIdGenerator();

            return
                new PSTFile(
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    new NIDDecoder(),
                    new ChangesTracker(),
                    CreateHeaderBasedStringEncoder(stream),
                    CreateNodeEntryFinder(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreateNIDBasedRowIndexReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreateNIDBasedTableContextReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreatePropertyIdToNameMap(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreatePropertyContextBasedPropertyReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    CreateTagBasedTableContextBasedPropertyReader(stream, cachedNodeEntries, dataBlockEntryCache),
                    unallocatedNodeIdGenerator);
        }
    }
}
