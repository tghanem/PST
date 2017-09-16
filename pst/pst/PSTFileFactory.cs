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
using pst.impl.ndb.bbt;
using pst.impl.ndb.cache;
using pst.impl.ndb.datatree;
using pst.impl.ndb.nbt;
using pst.impl.ndb.subnodebtree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
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

            return
                new PSTFile(
                    new EntryIdDecoder(
                        new NIDDecoder()),
                    CreateReadOnlyFolder(stream, cachedNodeEntries),
                    CreateReadOnlyMessage(stream, cachedNodeEntries),
                    CreateReadOnlyAttachment(
                        stream,
                        cachedNodeEntries,
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache),
                    CreatePropertyContextBasedReadOnlyComponent(
                        stream,
                        cachedNodeEntries,
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache),
                    CreateTagBasedTableContextBasedReadOnlyComponent(stream, cachedNodeEntries));
        }

        private static IReadOnlyFolder CreateReadOnlyFolder(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache)
        {
            return
                new ReadOnlyFolder(
                    CreateNodeEntryFinder(dataStream, nodeEntryCache),
                    CreateNIDBasedRowIndexReader(dataStream),
                    new NIDDecoder());
        }

        private static IReadOnlyMessage CreateReadOnlyMessage(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache)
        {
            return
                new ReadOnlyMessage(
                    CreateSubnodesEnumerator(dataStream),
                    CreateNIDBasedTableContextReader(dataStream),
                    CreateNIDBasedRowIndexReader(dataStream),
                    new NIDDecoder(),
                    CreateNodeEntryFinder(dataStream, nodeEntryCache));
        }

        private static IReadOnlyAttachment CreateReadOnlyAttachment(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache)
        {
            return
                new ReadOnlyAttachment(
                    new NIDDecoder(),
                    CreateNodeEntryFinder(
                        dataStream,
                        nodeEntryCache),
                    CreatePropertyContextBasedReadOnlyComponent(
                        dataStream,
                        nodeEntryCache,
                        numericalTaggedPropertyCache,
                        stringTaggedPropertyCache,
                        taggedPropertyCache));
        }

        private static IPropertyContextBasedReadOnlyComponent CreatePropertyContextBasedReadOnlyComponent(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache,
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache)
        {
            return
                new PropertyContextBasedReadOnlyComponentThatCachesThePropertyValue(
                    numericalTaggedPropertyCache,
                    stringTaggedPropertyCache,
                    taggedPropertyCache,
                    new PropertyContextBasedReadOnlyComponent(
                        CreateNodeEntryFinder(dataStream, nodeEntryCache),
                        CreatePropertyIdToNameMap(dataStream),
                        CreatePropertyContextBasedPropertyReader(dataStream)));
        }

        private static ITableContextBasedReadOnlyComponent<Tag> CreateTagBasedTableContextBasedReadOnlyComponent(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache)
        {
            return
                new TableContextBasedReadOnlyComponent<Tag>(
                    CreateNodeEntryFinder(dataStream, nodeEntryCache),
                    CreatePropertyIdToNameMap(dataStream),
                    CreateTagBasedTableContextBasedPropertyReader(dataStream));
        }

        private static ITableContextReader CreateNIDBasedTableContextReader(
            Stream stream)
        {
            return
                new TableContextReader<NID>(
                    new NIDDecoder(),
                    CreateNIDBasedRowIndexReader(stream),
                    CreateNIDBasedRowMatrixReader(stream));
        }

        private static PropertyNameToIdMap CreatePropertyIdToNameMap(
            Stream dataStream)
        {
            return
                new PropertyNameToIdMap(
                    new NAMEIDDecoder(),
                    CreatePropertyContextBasedPropertyReader(dataStream),
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
            Stream dataStream)
        {
            return
                new TableContextBasedPropertyReader<Tag>(
                    CreateTagBasedRowMatrixReader(dataStream),
                    CreatePropertyValueProcessor(dataStream));
        }

        private static IRowIndexReader<Tag> CreateTagBasedRowIndexReader(
            Stream dataStream)
        {
            return
                new RowIndexReader<Tag>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(dataStream),
                    CreateTagBasedBTreeOnHeapReader(dataStream),
                    new DataRecordToTCROWIDConverter());
        }

        private static IRowIndexReader<NID> CreateNIDBasedRowIndexReader(
            Stream dataStream)
        {
            return
                new RowIndexReader<NID>(
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateHeapOnNodeReader(dataStream),
                    CreateNIDBasedBTreeOnHeapReader(dataStream),
                    new DataRecordToTCROWIDConverter());
        }

        private static IPropertyContextBasedPropertyReader CreatePropertyContextBasedPropertyReader(
            Stream dataStream)
        {
            return
                new PropertyContextBasedPropertyReader(
                    CreatePropertyIdBasedBTreeOnHeapReader(dataStream),
                    CreatePropertyValueProcessor(dataStream));
        }

        private static IPropertyValueProcessor CreatePropertyValueProcessor(
            Stream dataStream)
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
                    CreateHeapOnNodeReader(dataStream),
                    CreateSubnodesEnumerator(dataStream),
                    CreateDataBlockEntryFinder(dataStream), 
                    new PropertyTypeMetadataProvider(),
                    CreateDataBlockReader(dataStream));
        }

        private static IRowMatrixReader<Tag> CreateTagBasedRowMatrixReader(
            Stream dataStream)
        {
            return
                new RowMatrixReader<Tag>(
                    CreateHeapOnNodeReader(dataStream),
                    new RowValuesExtractor(),
                    CreateSubnodesEnumerator(dataStream),
                    CreateTagBasedRowIndexReader(dataStream),
                    CreateDataBlockEntryFinder(dataStream), 
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateDataBlockReader(dataStream));
        }

        private static IRowMatrixReader<NID> CreateNIDBasedRowMatrixReader(
            Stream dataStream)
        {
            return
                new RowMatrixReader<NID>(
                    CreateHeapOnNodeReader(dataStream),
                    new RowValuesExtractor(),
                    CreateSubnodesEnumerator(dataStream),
                    CreateNIDBasedRowIndexReader(dataStream),
                    CreateDataBlockEntryFinder(dataStream), 
                    new HNIDDecoder(
                        new HIDDecoder(),
                        new NIDDecoder()),
                    new TCINFODecoder(
                        new HIDDecoder(),
                        new TCOLDESCDecoder()),
                    CreateDataBlockReader(dataStream));
        }

        private static IBTreeOnHeapReader<Tag> CreateTagBasedBTreeOnHeapReader(
            Stream dataStream)
        {
            return
                new BTreeOnHeapReader<Tag>(
                    new HIDDecoder(),
                    new TagDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream));
        }

        private static IBTreeOnHeapReader<NID> CreateNIDBasedBTreeOnHeapReader(
            Stream dataStream)
        {
            return
                new BTreeOnHeapReader<NID>(
                    new HIDDecoder(),
                    new NIDDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream));
        }

        private static IBTreeOnHeapReader<PropertyId> CreatePropertyIdBasedBTreeOnHeapReader(
            Stream dataStream)
        {
            return
                new BTreeOnHeapReader<PropertyId>(
                    new HIDDecoder(),
                    new PropertyIdDecoder(),
                    new BTHHEADERDecoder(
                        new HIDDecoder()),
                    CreateHeapOnNodeReader(dataStream));
        }

        private static IHeapOnNodeReader CreateHeapOnNodeReader(
            Stream dataStream)
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
                    CreateDataBlockEntryFinder(dataStream), 
                    CreateDataBlockReader(dataStream));
        }

        private static IDataBlockReader CreateDataBlockReader(
            Stream dataReader)
        {
            return
                new BlockIdBasedDataBlockReader(
                    new DataReader(dataReader),
                    CreateDataBlockEntryFinder(dataReader));
        }

        private static IDataBlockEntryFinder CreateDataBlockEntryFinder(
            Stream dataReader)
        {
            return
                new DataBlockEntryFinder(
                    new DataReader(dataReader),
                    CreateHeaderDecoder(),
                    new BIDsFromInternalDataBlockExtractor(
                        new BIDDecoder()),
                    new InternalDataBlockLoader(
                        new InternalDataBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder())),
                        new DataReader(dataReader)),
                    CreateBlockBTreeEntryFinder(dataReader));
        }

        private static IBTreeEntryFinder<BID, LBBTEntry, BREF> CreateBlockBTreeEntryFinder(
            Stream dataStream)
        {
            return
                new BTreeEntryFinder<BID, LBBTEntry, IBBTEntry, BREF, BTPage>(
                    new FuncBasedExtractor<LBBTEntry, BID>(
                        entry => entry.BlockReference.BlockId),
                    new FuncBasedExtractor<IBBTEntry, BID>(
                        entry => entry.Key),
                    new FuncBasedExtractor<IBBTEntry, BREF>(
                        entry => entry.ChildPageBlockReference),
                    new IBBTEntriesFromBTPageExtractor(
                        new IBBTEntryDecoder(
                            new BIDDecoder(),
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new LBBTEntriesFromBTPageExtractor(
                        new LBBTEntryDecoder(
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new FuncBasedExtractor<BTPage, int>(
                        page => page.PageLevel),
                    new BTPageLoader(
                        new DataReader(dataStream),
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder()))));
        }

        private static INodeEntryFinder CreateNodeEntryFinder(
            Stream dataStream,
            ICache<NodePath, NodeEntry> nodeEntryCache)
        {
            return
                new NodeEntryFinderThatCachesTheNodeEntry(
                    nodeEntryCache,
                    new NodeEntryFinder(
                        CreateNIDToLNBTEntryMapper(dataStream),
                        CreateSubnodesEnumerator(dataStream)));
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
            Stream dataStream)
        {
            return
                new SubNodesEnumerator(
                    new SubnodeBTreeBlockLevelDecider(
                        CreateDataBlockReader(dataStream)),
                    new SubnodeBlockLoader(
                        new SubnodeBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder())),
                        CreateDataBlockReader(dataStream)),
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
