using pst.encodables.ndb;
using pst.impl;
using pst.impl.decoders;
using pst.impl.decoders.ndb;
using pst.impl.messaging;
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

            var numericalTaggedPropertyCache =
                new DictionaryBasedCache<NumericalTaggedPropertyPath, PropertyValue>();

            var stringTaggedPropertyCache =
                new DictionaryBasedCache<StringTaggedPropertyPath, PropertyValue>();

            var taggedPropertyCache =
                new DictionaryBasedCache<TaggedPropertyPath, PropertyValue>();

            var dataBlockEntryCache =
                new DictionaryBasedCache<BID, DataBlockEntry>();

            return
                new PSTFile(
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    CreateReadOnlyFolder(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache),
                    CreateReadOnlyMessage(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache),
                    CreateReadOnlyAttachment(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache,
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache),
                    CreatePropertyContextBasedReadOnlyComponent(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache,
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache),
                    CreateTagBasedTableContextBasedReadOnlyComponent(
                        stream,
                        cachedNodeEntries,
                        dataBlockEntryCache));
        }

        private static IReadOnlyFolder CreateReadOnlyFolder(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new ReadOnlyFolder(
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
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache)
        {
            return
                new ReadOnlyAttachment(
                    new NIDDecoder(),
                    CreateNodeEntryFinder(
                        dataStream,
                        nodeEntryCache,
                        dataBlockEntryCache),
                    CreatePropertyContextBasedReadOnlyComponent(
                        dataStream,
                        nodeEntryCache,
                        dataBlockEntryCache, numericalTaggedPropertyCache, stringTaggedPropertyCache, taggedPropertyCache));
        }
    }
}
