using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.impl.btree;
using pst.impl.decoders.ltp.bth;
using pst.impl.decoders.ltp.hn;
using pst.impl.decoders.primitives;
using pst.impl.ltp.bth;
using pst.interfaces.btree;
using pst.interfaces.ltp.hn;

namespace pst.impl.ltp.pc
{
    class PropertyContext
    {
        private const int KeySize = 2;
        private const int DataSize = 6;

        private readonly IBTreeKeyFinder<DataRecord, PropertyId> btreeKeyFinder;
        
        public PropertyContext(IHeapOnNodeItemLoader hnItemLoader, HID rootNode, int treeDepth)
        {
            btreeKeyFinder =
                new KnownDepthBTreeKeyFinder<BTreeOnHeapNode, HID, IndexRecord, DataRecord, PropertyId>(
                    new BTreeNodeKeyLocator<BTreeOnHeapNode, IndexRecord, PropertyId>(
                        new ComparerThatFindsTheFirstKeyThatIsLargerThanTheReferenceKey<IndexRecord, PropertyId>(
                            new PropertyIdFromIndexRecordExtractor(
                                new Int32Decoder())),
                        new IndexRecordsFromBTreeOnHeapNodeExtractor(
                            new IndexRecordDecoder(
                                new HIDDecoder(
                                    new Int32Decoder()),
                                KeySize),
                            KeySize)),
                    new BTreeNodeKeyLocator<BTreeOnHeapNode, DataRecord, PropertyId>(
                        new ComparerThatFindsTheFirstKeyThatMatchesTheReferenceKey<DataRecord, PropertyId>(
                            new PropertyIdFromDataRecordExtractor(
                                new Int32Decoder())),
                        new DataRecordsFromBTreeOnHeapNodeExtractor(
                            new DataRecordDecoder(
                                KeySize,
                                DataSize),
                            KeySize,
                            DataSize)),
                    new BTreeOnHeapNodeLoader(
                        hnItemLoader),
                    rootNode,
                    new HIDFromIndexRecordExtractor(),
                    treeDepth);
        }
    }
}
