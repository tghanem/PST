using pst.encodables;
using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders;
using pst.impl.decoders.ndb;
using pst.impl.messaging;
using pst.impl.messaging.cache;
using pst.interfaces;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        public static PSTFile Open(Stream stream)
        {
            var cachedNodeEntries =
                new DictionaryBasedCache<NodePath, NodeEntry>();

            var dataBlockEntryCache =
                new DictionaryBasedCache<BID, DataBlockEntry>();

            var propertyContextBasedNumericalTaggedPropertyCache =
                new DictionaryBasedCache<NumericalTaggedPropertyPath, PropertyContextBasedCachedPropertyState>();

            var propertyContextBasedStringTaggedPropertyCache =
                new DictionaryBasedCache<StringTaggedPropertyPath, PropertyContextBasedCachedPropertyState>();

            var propertyContextBasedTaggedPropertyCache =
                new DictionaryBasedCache<TaggedPropertyPath, PropertyContextBasedCachedPropertyState>();

            var tableContextBasedNumericalTaggedPropertyCache =
                new DictionaryBasedCache<NumericalTaggedPropertyPath, TableContextBasedCachedPropertyState<Tag>>();

            var tableContextBasedStringTaggedPropertyCache =
                new DictionaryBasedCache<StringTaggedPropertyPath, TableContextBasedCachedPropertyState<Tag>>();

            var tableContextBasedTaggedPropertyCache =
                new DictionaryBasedCache<TaggedPropertyPath, TableContextBasedCachedPropertyState<Tag>>();

            return
                new PSTFile(
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    CreateFolder(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache),
                    CreateHeaderBasedStringEncoder(stream), 
                    CreateReadOnlyMessage(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache),
                    CreateReadOnlyAttachment(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache,
                        propertyContextBasedNumericalTaggedPropertyCache,
                        propertyContextBasedStringTaggedPropertyCache,
                        propertyContextBasedTaggedPropertyCache),
                    CreatePropertyContextBasedComponent(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache,
                        propertyContextBasedNumericalTaggedPropertyCache,
                        propertyContextBasedStringTaggedPropertyCache,
                        propertyContextBasedTaggedPropertyCache),
                    CreateTagBasedTableContextBasedReadOnlyComponent(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache,
                        tableContextBasedNumericalTaggedPropertyCache,
                        tableContextBasedStringTaggedPropertyCache,
                        tableContextBasedTaggedPropertyCache));
        }

        private static IFolder CreateFolder(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new impl.messaging.Folder(
                    CreateNIDBasedRowIndexReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new NIDDecoder());
        }

        private static IReadOnlyMessage CreateReadOnlyMessage(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new ReadOnlyMessage(
                    CreateNIDBasedTableContextReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateNIDBasedRowIndexReader(dataStream, nodeEntryCache, dataBlockEntryCache),
                    new NIDDecoder(),
                    CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IReadOnlyAttachment CreateReadOnlyAttachment(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache,
            ICache<NumericalTaggedPropertyPath, PropertyContextBasedCachedPropertyState> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyContextBasedCachedPropertyState> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyContextBasedCachedPropertyState> taggedPropertyCache)
        {
            return
                new ReadOnlyAttachment(
                    new NIDDecoder(),
                    CreateNodeEntryFinder(
                        dataStream,
                        nodeEntryCache,
                        dataBlockEntryCache),
                    CreatePropertyContextBasedComponent(
                        dataStream,
                        nodeEntryCache,
                        dataBlockEntryCache, numericalTaggedPropertyCache, stringTaggedPropertyCache, taggedPropertyCache));
        }
    }
}
