using pst.encodables.ltp.hn;
using pst.utilities;
using pst.interfaces.ndb;
using pst.interfaces.ltp.hn;
using pst.core;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeItemLoader : IHeapOnNodeItemLoader
    {
        private readonly IOrderedDataBlockCollection nodeDataBlockCollection;

        private readonly IHeapOnNodeItemsExtractor heapItemsExtractor;

        public HeapOnNodeItemLoader(IOrderedDataBlockCollection nodeDataBlockCollection, IHeapOnNodeItemsExtractor heapItemsExtractor)
        {
            this.nodeDataBlockCollection = nodeDataBlockCollection;
            this.heapItemsExtractor = heapItemsExtractor;
        }

        public Maybe<BinaryData> Load(HID id)
        {
            var dataBlock =
                nodeDataBlockCollection.GetDataBlockAt(id.BlockIndex);

            var items =
                heapItemsExtractor.Extract(dataBlock.Data, id.BlockIndex);

            return items[id.Index - 1];
        }
    }
}
