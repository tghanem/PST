using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.impl.decoders.primitives;
using pst.impl.io;
using pst.impl.ltp.pc;
using pst.impl.ndb;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using pst.utilities;
using System.IO;

namespace pst
{
    public class PSTFile
    {
        private readonly IBTreeKeyFinder<LNBTEntry, NID> nodeBTree;
        private readonly IOrderedDataBlockCollectionLoader nodeOrderedDataBlockCollectionLoader;

        public PSTFile(Stream stream)
        {
            var dataReader =
                new StreamBasedDataReader(stream);

            var header =
                Factory
                .HeaderDecoder
                .Decode(dataReader .Read(IB.Zero, 564));

            nodeBTree =
                Factory
                .CreateNodeBTreeKeyFinder(dataReader, header.Root.NBTRootPage);

            nodeOrderedDataBlockCollectionLoader =
                new OrderedNodeDataBlockCollectionLoader(
                    dataReader,
                    Factory
                    .CreateBlockBTreeKeyFinder(dataReader, header.Root.BBTRootPage));
        }

        public PSTFile(string filePath)
            : this(File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
        }

        public MessageStore GetMessageStore()
        {
            var lnbtEntry =
                nodeBTree.Find(new NID(0x21));

            var orderedDataBlockCollection =
                nodeOrderedDataBlockCollectionLoader
                .Load(lnbtEntry.Value.DataBlockId);

            var hnHDR =
                Factory
                .HeapOnNodeHeaderDecoder
                .Decode(orderedDataBlockCollection.Value.GetDataBlockAt(0).Data);

            var heapOnNodeItemLoader =
                Factory
                .CreateHeapOnNodeItemLoader(orderedDataBlockCollection.Value);

            var rootHeapItem =
                heapOnNodeItemLoader
                .Load(hnHDR.UserRoot);

            var bthHeader =
                Factory
                .BTreeOnHeapHeaderDecoder
                .Decode(rootHeapItem.Value);

            var propertyContext =
                Factory
                .CreateBTreeOnHeap(
                    new PropertyIdFromIndexRecordExtractor(
                        new Int32Decoder()),
                    new PropertyIdFromDataRecordExtractor(
                        new Int32Decoder()),
                    heapOnNodeItemLoader,
                    bthHeader);
            
            return
                new MessageStore(
                    new PropertyContext(
                        propertyContext,
                        new PropertyTypeMetadataProvider(),
                        Factory.PropertyTypeDecoder,
                        heapOnNodeItemLoader,
                        Factory.HIDDecoder,
                        Factory.NIDDecoder));
        }
    }
}
