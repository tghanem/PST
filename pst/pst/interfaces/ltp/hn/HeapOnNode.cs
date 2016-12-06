using pst.encodables.ltp.hn;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.hn
{
    class HeapOnNode : Dictionary<HID, BinaryData>
    {
        private readonly HID rootHID;

        public HeapOnNode(HID rootHID)
        {
            this.rootHID = rootHID;
        }

        public void FillFrom(IDictionary<HID, BinaryData> heap)
        {
            foreach (var item in heap)
            {
                Add(item.Key, item.Value);
            }
        }

        public BinaryData Root
            =>
            rootHID == HID.Zero
            ? BinaryData.Empty()
            : this[rootHID];
    }
}
