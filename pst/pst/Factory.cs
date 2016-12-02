using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.btree;
using pst.impl.decoders.ltp.bth;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.messaging;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.btree;
using pst.impl.decoders.primitives;
using pst.impl.ltp.bth;
using pst.impl.ltp.hn;
using pst.impl.ndb;
using pst.impl.ndb.bbt;
using pst.impl.ndb.nbt;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using System;

namespace pst
{
    static class Factory
    {
        public static IDecoder<Header> HeaderDecoder;
        public static IDecoder<HNHDR> HeapOnNodeHeaderDecoder;
        public static IDecoder<BTHHEADER> BTreeOnHeapHeaderDecoder;
        public static IDecoder<HID> HIDDecoder;
        public static IDecoder<NID> NIDDecoder;
        public static IDecoder<PropertyType> PropertyTypeDecoder;

        static Factory()
        {
            HeaderDecoder =
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

            HeapOnNodeHeaderDecoder =
                new HNHDRDecoder(
                    new Int32Decoder(),
                    new HIDDecoder(
                        new Int32Decoder()));

            BTreeOnHeapHeaderDecoder =
                new BTHHEADERDecoder(
                    new Int32Decoder(),
                    new HIDDecoder(
                        new Int32Decoder()));

            HIDDecoder =
                new HIDDecoder(
                    new Int32Decoder());

            NIDDecoder =
                new NIDDecoder();

            PropertyTypeDecoder =
                new PropertyTypeDecoder(
                    new Int32Decoder());
        }

        public static IHeapOnNodeItemLoader CreateHeapOnNodeItemLoader(IOrderedDataBlockCollection orderedNodeBlockCollection)
        {
            return
                new HeapOnNodeItemLoader(
                    orderedNodeBlockCollection,
                    new HeapItemsExtractor(
                        new HNHDRDecoder(
                            new Int32Decoder(),
                            new HIDDecoder(
                                new Int32Decoder())),
                        new HNPAGEMAPDecoder(
                            new Int32Decoder()),
                        new Int32Decoder()));
        }

        public static IBTreeKeyFinder<DataRecord, TReferenceKey> CreateBTreeOnHeap<TReferenceKey>(
            IExtractor<IndexRecord, TReferenceKey> referenceKeyFromIndexRecordExtractor,
            IExtractor<DataRecord, TReferenceKey> referenceKeyFromDataRecordExtractor,
            IHeapOnNodeItemLoader hnItemLoader,
            BTHHEADER bthHeader)
            where TReferenceKey : class, IComparable<TReferenceKey>, IEquatable<TReferenceKey>
        {
            return
                new KnownDepthBTreeKeyFinder<BTreeOnHeapNode, HID, IndexRecord, DataRecord, TReferenceKey>(
                    new BTreeNodeKeyLocator<BTreeOnHeapNode, IndexRecord, TReferenceKey>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<IndexRecord, TReferenceKey>(
                            referenceKeyFromIndexRecordExtractor),
                        new IndexRecordsFromBTreeOnHeapNodeExtractor(
                            new IndexRecordDecoder(
                                new HIDDecoder(
                                    new Int32Decoder()),
                                bthHeader.Key),
                            bthHeader.Key)),
                    new BTreeNodeKeyLocator<BTreeOnHeapNode, DataRecord, TReferenceKey>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<DataRecord, TReferenceKey>(
                            referenceKeyFromDataRecordExtractor),
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

        public static IBTreeKeyFinder<LBBTEntry, BID> CreateBlockBTreeKeyFinder(IDataReader dataReader, BREF rootNodeReference)
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

        public static IBTreeKeyFinder<LNBTEntry, NID> CreateNodeBTreeKeyFinder(IDataReader dataReader, BREF rootNodeReference)
        {
            return
                new BTreeKeyFinder<BTPage, BREF, INBTEntry, LNBTEntry, NID>(
                    new BTreeNodeKeyLocator<BTPage, INBTEntry, NID>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<INBTEntry, NID>(
                            new NIDFromINBTEntryExtractor()),
                        new INBTEntriesFromBTPageExtractor(
                            new INBTEntryDecoder(
                                new NIDDecoder(),
                                new BREFDecoder(
                                    new BIDDecoder(),
                                    new IBDecoder())))),
                    new BTreeNodeKeyLocator<BTPage, LNBTEntry, NID>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<LNBTEntry, NID>(
                            new NIDFromLNBTEntryExtractor()),
                        new LNBTEntriesFromBTPageExtractor(
                            new LNBTEntryDecoder(
                                new NIDDecoder(),
                                new BIDDecoder()))),
                    new BTPageLoader(
                        dataReader,
                        new BTPageDecoder(
                            new Int32Decoder(),
                            new PageTrailerDecoder(
                                new Int32Decoder(),
                                new BIDDecoder()))),
                    rootNodeReference,
                    new BREFFromINBTEntryExtractor(),
                    new PageLevelFromBTPageExtractor());
        }
    }
}
