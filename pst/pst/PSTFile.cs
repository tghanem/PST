using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.btree;
using pst.impl.decoders.ltp.bth;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.btree;
using pst.impl.decoders.primitives;
using pst.impl.io;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.impl.ltp.pc;
using pst.impl.ndb;
using pst.impl.ndb.bbt;
using pst.impl.ndb.nbt;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public class PSTFile
    {
        private readonly NodeBTree nodeBTree;
        private readonly IOrderedDataBlockCollectionLoader nodeOrderedDataBlockCollectionLoader;

        public PSTFile(string filePath)
        {
            var fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);

            var dataReader = new StreamBasedDataReader(fileStream);

            var headerDecoder =
                new HeaderDecoder(
                    new Int32Decoder(),
                    new RootDecoder(
                        new Int32Decoder(),
                        new Int64Decoder(),
                        new BREFDecoder(
                             new BIDDecoder(),
                             new IBDecoder())),
                    new BIDDecoder(),
                    new NIDDecoder());

            var header =
                headerDecoder.Decode(dataReader.Read(IB.Zero, 564));

            nodeBTree =
                new NodeBTree(dataReader, header.Root.NBTRootPage);

            nodeOrderedDataBlockCollectionLoader =
                new OrderedNodeDataBlockCollectionLoader(
                    CreateBlockBTreeKeyFinder(
                        dataReader,
                        header.Root.BBTRootPage),
                    new OrderedDataBlockCollectionFactory(
                        dataReader));
        }

        public MessageStore GetMessageStore()
        {
            var lnbtEntry =
                nodeBTree.Find(new NID(0x21));

            var orderedDataBlockCollection =
                nodeOrderedDataBlockCollectionLoader.Load(lnbtEntry.DataBlockId);

            var hnHDRDecoder =
                new HNHDRDecoder(
                    new Int32Decoder(),
                    new HIDDecoder(
                        new Int32Decoder()));

            var hnHDR =
                hnHDRDecoder.Decode(orderedDataBlockCollection.Value.GetDataBlockAt(0).Data);

            var heapOnNodeItemLoader =
                new HeapOnNodeItemLoader(
                    orderedDataBlockCollection.Value,
                    new HeapItemsExtractor(
                        new HNHDRDecoder(
                            new Int32Decoder(),
                            new HIDDecoder(
                                new Int32Decoder())),
                        new HNPAGEMAPDecoder(
                            new Int32Decoder()),
                        new Int32Decoder()));

            var rootHeapItem =
                heapOnNodeItemLoader.Load(hnHDR.UserRoot);

            var bthHeaderDecoder =
                new BTHHEADERDecoder(
                    new Int32Decoder(),
                    new HIDDecoder(
                        new Int32Decoder()));

            var bthHeader =
                bthHeaderDecoder.Decode(rootHeapItem.Value);

            var propertyContext =
                CreateBTreeOnHeap(
                    heapOnNodeItemLoader,
                    bthHeader);

            return
                new MessageStore(
                    propertyContext,
                    heapOnNodeItemLoader,
                    new HIDDecoder(
                        new Int32Decoder()));
        }

        private IBTreeKeyFinder<DataRecord, PropertyId> CreateBTreeOnHeap(IHeapOnNodeItemLoader hnItemLoader,  BTHHEADER bthHeader)
        {
            return
                new KnownDepthBTreeKeyFinder<BTreeOnHeapNode, HID, IndexRecord, DataRecord, PropertyId>(
                    new BTreeNodeKeyLocator<BTreeOnHeapNode, IndexRecord, PropertyId>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<IndexRecord, PropertyId>(
                            new PropertyIdFromIndexRecordExtractor(
                                new Int32Decoder())),
                        new IndexRecordsFromBTreeOnHeapNodeExtractor(
                            new IndexRecordDecoder(
                                new HIDDecoder(
                                    new Int32Decoder()),
                                bthHeader.Key),
                            bthHeader.Key)),
                    new BTreeNodeKeyLocator<BTreeOnHeapNode, DataRecord, PropertyId>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<DataRecord, PropertyId>(
                            new PropertyIdFromDataRecordExtractor(
                                new Int32Decoder())),
                        new DataRecordsFromBTreeOnHeapNodeExtractor(
                            new DataRecordDecoder(
                                bthHeader.Key,
                                bthHeader.SizeOfDataValue),
                            bthHeader.Key,
                            bthHeader.SizeOfDataValue)),
                    new BTreeOnHeapNodeLoader(
                        hnItemLoader),
                    bthHeader.Root,
                    new HIDFromIndexRecordExtractor(),
                    bthHeader.IndexDepth);
        }

        private IBTreeKeyFinder<LBBTEntry, BID> CreateBlockBTreeKeyFinder(IDataReader dataReader, BREF rootNodeReference)
        {
            return
                new BTreeKeyFinder<BTPage, BREF, IBBTEntry, LBBTEntry, BID>(
                    new BTreeNodeKeyLocator<BTPage, IBBTEntry, BID>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<IBBTEntry, BID>(
                            new BIDFromIBBTEntryExtractor()),
                        new IBBTEntriesFromBTPageExtractor(
                            new IBBTEntryDecoder(
                                new BIDDecoder(),
                                new BREFDecoder(
                                    new BIDDecoder(),
                                    new IBDecoder())))),
                    new BTreeNodeKeyLocator<BTPage, LBBTEntry, BID>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<LBBTEntry, BID>(
                            new BIDFromLBBTEntryExtractor()),
                        new LBBTEntriesFromBTPageExtractor(
                            new LBBTEntryDecoder(
                                new BREFDecoder(
                                    new BIDDecoder(),
                                    new IBDecoder()),
                                new Int32Decoder()))),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new Int32Decoder(),
                            new PageTrailerDecoder(
                                new Int32Decoder(),
                                new BIDDecoder()))),
                    rootNodeReference,
                    new BREFFromIBBTEntryExtractor(),
                    new PageLevelFromBTPageExtractor());
        }
    }
}
