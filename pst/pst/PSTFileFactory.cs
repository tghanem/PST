using pst.encodables;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl;
using pst.impl.btree;
using pst.impl.converters;
using pst.impl.decoders;
using pst.impl.decoders.ltp.bth;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.ltp.tc;
using pst.impl.decoders.messaging;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.blocks;
using pst.impl.decoders.ndb.blocks.data;
using pst.impl.decoders.ndb.blocks.subnode;
using pst.impl.decoders.ndb.btree;
using pst.impl.io;
using pst.impl.ltp;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.impl.ltp.pc;
using pst.impl.ltp.tc;
using pst.impl.messaging;
using pst.impl.messaging.cache;
using pst.impl.ndb;
using pst.impl.ndb.cache;
using pst.impl.ndb.nbt;
using pst.impl.ndb.subnodebtree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ltp;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;
using pst.interfaces.ndb;
using pst.utilities;
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
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache,
                        dataBlockEntryCache),
                    CreatePropertyContextBasedReadOnlyComponent(
                        stream,
                        cachedNodeEntries,
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache,
                        dataBlockEntryCache),
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
                    CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreateNIDBasedRowIndexReader(dataStream, dataBlockEntryCache),
                    new NIDDecoder());
        }

        private static IReadOnlyMessage CreateReadOnlyMessage(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new ReadOnlyMessage(
                    CreateSubnodesEnumerator(dataStream, dataBlockEntryCache),
                    CreateNIDBasedTableContextReader(dataStream, dataBlockEntryCache),
                    CreateNIDBasedRowIndexReader(dataStream, dataBlockEntryCache),
                    new NIDDecoder(),
                    CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache));
        }

        private static IReadOnlyAttachment CreateReadOnlyAttachment(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
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
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache,
                        dataBlockEntryCache));
        }

        private static IPropertyContextBasedReadOnlyComponent CreatePropertyContextBasedReadOnlyComponent(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyContextBasedReadOnlyComponentThatCachesThePropertyValue(
                    numericalTaggedPropertyCache,
                    stringTaggedPropertyCache,
                    taggedPropertyCache,
                    new PropertyContextBasedReadOnlyComponent(
                        CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                        CreatePropertyIdToNameMap(dataStream, dataBlockEntryCache),
                        CreatePropertyContextBasedPropertyReader(dataStream, dataBlockEntryCache)));
        }

        private static ITableContextBasedReadOnlyComponent<Tag> CreateTagBasedTableContextBasedReadOnlyComponent(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new TableContextBasedReadOnlyComponent<Tag>(
                    CreateNodeEntryFinder(dataStream, nodeEntryCache, dataBlockEntryCache),
                    CreatePropertyIdToNameMap(dataStream, dataBlockEntryCache),
                    CreateTagBasedTableContextBasedPropertyReader(dataStream, dataBlockEntryCache));
        }

        private static ITableContextReader CreateNIDBasedTableContextReader(
            Stream stream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new TableContextReader<NID>(
                    new NIDDecoder(),
                    CreateNIDBasedRowIndexReader(stream, dataBlockEntryCache),
                    CreateNIDBasedRowMatrixReader(stream, dataBlockEntryCache));
        }

        private static PropertyNameToIdMap CreatePropertyIdToNameMap(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyNameToIdMap(
                    new NAMEIDDecoder(),
                    CreatePropertyContextBasedPropertyReader(dataStream, dataBlockEntryCache),
                    CreateNIDToLNBTEntryMapper(dataStream));
        }

        private static IDecoder<Header> CreateHeaderDecoder()
        {
            return
                new HeaderDecoder(
                    new RootDecoder(
                        new BREFDecoder(
                            new BIDDecoder(),
                            new IBDecoder())),
                    new BIDDecoder(),
                    new NIDDecoder());
        }

        private static IBlockDataDeObfuscator CreateBlockDataDeObfuscator(
            Stream dataStream)
        {
            return
                new EncodingThatDetectsTypeFromTheHeader(
                    new DataReader(dataStream),
                    CreateHeaderDecoder(),
                    new PermutativeEncoding(),
                    new CyclicEncoding(),
                    new NoEncoding());
        }

        private static ITableContextBasedPropertyReader<Tag> CreateTagBasedTableContextBasedPropertyReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new TableContextBasedPropertyReader<Tag>(
                    CreateTagBasedRowMatrixReader(dataStream, dataBlockEntryCache),
                    CreatePropertyValueProcessor(dataStream, dataBlockEntryCache));
        }

        private static IRowIndexReader<Tag> CreateTagBasedRowIndexReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowIndexReader<Tag>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache),
                    CreateTagBasedBTreeOnHeapReader(dataStream, dataBlockEntryCache),
                    new DataRecordToTCROWIDConverter());
        }

        private static IRowIndexReader<NID> CreateNIDBasedRowIndexReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowIndexReader<NID>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache),
                    CreateNIDBasedBTreeOnHeapReader(dataStream, dataBlockEntryCache),
                    new DataRecordToTCROWIDConverter());
        }

        private static IPropertyContextBasedPropertyReader CreatePropertyContextBasedPropertyReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyContextBasedPropertyReader(
                    CreatePropertyIdBasedBTreeOnHeapReader(dataStream, dataBlockEntryCache),
                    CreatePropertyValueProcessor(dataStream, dataBlockEntryCache));
        }

        private static IPropertyValueProcessor CreatePropertyValueProcessor(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new PropertyValueProcessor(
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new ExternalDataBlockDecoder(
                        new BlockTrailerDecoder(
                            new BIDDecoder()),
                        CreateBlockDataDeObfuscator(dataStream)),
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache),
                    CreateSubnodesEnumerator(dataStream, dataBlockEntryCache),
                    CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                    new PropertyTypeMetadataProvider(),
                    CreateDataBlockReader(dataStream, dataBlockEntryCache));
        }

        private static IRowMatrixReader<Tag> CreateTagBasedRowMatrixReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowMatrixReader<Tag>(
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache),
                    new RowValuesExtractor(),
                    CreateSubnodesEnumerator(dataStream, dataBlockEntryCache),
                    CreateTagBasedRowIndexReader(dataStream, dataBlockEntryCache),
                    CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateDataBlockReader(dataStream, dataBlockEntryCache));
        }

        private static IRowMatrixReader<NID> CreateNIDBasedRowMatrixReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new RowMatrixReader<NID>(
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache),
                    new RowValuesExtractor(),
                    CreateSubnodesEnumerator(dataStream, dataBlockEntryCache),
                    CreateNIDBasedRowIndexReader(dataStream, dataBlockEntryCache),
                    CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateDataBlockReader(dataStream, dataBlockEntryCache));
        }

        private static IBTreeOnHeapReader<Tag> CreateTagBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new BTreeOnHeapReader<Tag>(
                    new HIDDecoder(),
                    new TagDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache));
        }

        private static IBTreeOnHeapReader<NID> CreateNIDBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new BTreeOnHeapReader<NID>(
                    new HIDDecoder(),
                    new NIDDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache));
        }

        private static IBTreeOnHeapReader<PropertyId> CreatePropertyIdBasedBTreeOnHeapReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new BTreeOnHeapReader<PropertyId>(
                    new HIDDecoder(),
                    new PropertyIdDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream, dataBlockEntryCache));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new HeapOnNodeReader(
                    new HNHDRDecoder(
                        new HIDDecoder()),
                    new HNPAGEHDRDecoder(),
                    new HNPAGEMAPDecoder(),
                    new HNBITMAPHDRDecoder(),
                    CreateBlockDataDeObfuscator(dataStream),
                    new HeapOnNodeItemsLoader(),
                    CreateDataBlockEntryFinder(dataStream, dataBlockEntryCache),
                    CreateDataBlockReader(dataStream, dataBlockEntryCache));
        }



        private static INodeEntryFinder CreateNodeEntryFinder(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new NodeEntryFinderThatCachesTheNodeEntry(
                    nodeEntryCache,
                    new NodeEntryFinder(
                        CreateNIDToLNBTEntryMapper(dataStream),
                        CreateSubnodesEnumerator(dataStream, dataBlockEntryCache)));
        }

        private static NIDToLNBTEntryMapper CreateNIDToLNBTEntryMapper(
            Stream dataStream)
        {
            return
                new NIDToLNBTEntryMapper(
                    new DataReader(dataStream),
                    CreateHeaderDecoder(),
                    CreateNodeBTreeEntryFinder(dataStream));
        }

        private static ISubNodesEnumerator CreateSubnodesEnumerator(
            Stream dataStream,
            ICache<BID, DataBlockEntry> dataBlockEntryCache)
        {
            return
                new SubNodesEnumerator(
                    new SubnodeBTreeBlockLevelDecider(
                        CreateDataBlockReader(dataStream, dataBlockEntryCache)),
                    new SubnodeBlockLoader(
                        new SubnodeBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder())),
                        CreateDataBlockReader(dataStream, dataBlockEntryCache)),
                    new SIEntriesFromSubnodeBlockExtractor(
                        new SIEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new SLEntriesFromSubnodeBlockExtractor(
                        new SLEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())));
        }

        private static IBTreeEntryFinder<NID, LNBTEntry, BREF> CreateNodeBTreeEntryFinder(
            Stream dataStream)
        {
            return
                new BTreeEntryFinder<NID, LNBTEntry, INBTEntry, BREF, BTPage>(
                    new FuncBasedExtractor<LNBTEntry, NID>(
                        entry => entry.NodeId),
                    new FuncBasedExtractor<INBTEntry, NID>(
                        entry => entry.Key),
                    new FuncBasedExtractor<INBTEntry, BREF>(
                        entry => entry.ChildPageBlockReference),
                    new INBTEntriesFromBTPageExtractor(
                        new INBTEntryDecoder(
                            new NIDDecoder(),
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new LNBTEntriesFromBTPageExtractor(
                        new LNBTEntryDecoder(
                            new NIDDecoder(),
                            new BIDDecoder())),
                    new FuncBasedExtractor<BTPage, int>(
                        page => page.PageLevel),
                    new BTPageLoader(
                        new DataReader(dataStream),
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }
    }
}
