using pst.encodables.ltp.hn;
using pst.utilities;
using System.Collections.Generic;
using System.Linq;

namespace pst.interfaces.ltp.hn
{
    class HeapOnNode
    {
        private readonly IDictionary<HID, BinaryData> heap;
        private readonly HID rootHID;

        public HeapOnNode(HID rootHID, IDictionary<HID, BinaryData> heap)
        {
            this.heap = heap;
            this.rootHID = rootHID;
        }

        public BinaryData GetItem(HID hid) => heap[hid];

        public HeapOnNode ChangeRoot(HID newRootHID)
        {
            return new HeapOnNode(newRootHID, heap);
        }

        public HeapOnNode Append(IDictionary<HID, BinaryData> additionalHeap)
        {
            return
                new HeapOnNode(
                    rootHID,
                    heap
                    .Concat(additionalHeap)
                    .ToDictionary(
                        p => p.Key,
                        p => p.Value));
        }

        public BinaryData Root
            =>
            rootHID == HID.Zero
            ? BinaryData.Empty()
            : heap[rootHID];
    }
}
