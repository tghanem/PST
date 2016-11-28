using pst.interfaces.ltp;
using pst.encodables.ltp.hn;
using pst.utilities;
using pst.interfaces.ndb;

namespace pst.impl.ltp
{
    class HeapOnNodeItemLoader : IHeapOnNodeItemLoader
    {
        private readonly IOrderedDataBlockCollection nodeDataBlockCollection;

        private readonly IHeapItemsExtractor heapItemsExtractor;

        public HeapOnNodeItemLoader(IOrderedDataBlockCollection nodeDataBlockCollection, IHeapItemsExtractor heapItemsExtractor)
        {
            this.nodeDataBlockCollection = nodeDataBlockCollection;
            this.heapItemsExtractor = heapItemsExtractor;
        }

        public BinaryData Load(HID id)
        {
            var dataBlock =
                nodeDataBlockCollection.GetDataBlockAt(id.BlockIndex);

            var items =
                heapItemsExtractor.Extract(dataBlock.Data, id.BlockIndex);

            return items[id.Index - 1];
        }
    }
}
