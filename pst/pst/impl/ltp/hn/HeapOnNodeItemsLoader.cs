using pst.encodables.ltp.hn;
using pst.interfaces.ltp.hn;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeItemsLoader : IHeapOnNodeItemsLoader
    {
        public IDictionary<HID, BinaryData> Load(int pageIndex, HNPAGEMAP pageMap, BinaryData pageData)
        {
            var items = new Dictionary<HID, BinaryData>();

            var offsets = GetOffsetsFromPageMap(pageMap);

            for (int i = 0; i < offsets.Length - 1; i++)
            {
                var itemData = pageData.Take(offsets[i], offsets[i + 1] - offsets[i]);

                var hid = new HID(Constants.NID_TYPE_HID, i + 1, pageIndex);

                items.Add(hid, itemData);
            }

            return items;
        }

        private int[] GetOffsetsFromPageMap(HNPAGEMAP pageMap)
        {
            var offsets = new List<int>();

            for (int i = 0; i <= pageMap.AllocationCount; i++)
            {
                var offset = pageMap.AllocationTable.Take(i * 2, 2);

                offsets.Add(offset.ToInt32());
            }

            return offsets.ToArray();
        }
    }
}
