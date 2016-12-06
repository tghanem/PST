using pst.interfaces.ltp.hn;
using System.Collections.Generic;
using pst.encodables.ltp.hn;
using pst.utilities;
using pst.interfaces;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeItemsLoader : IHeapOnNodeItemsLoader
    {
        private readonly IDecoder<int> int32Decoder;

        public HeapOnNodeItemsLoader(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public IDictionary<HID, BinaryData> Load(int pageIndex, HNPAGEMAP pageMap, BinaryData pageData)
        {
            var items = new Dictionary<HID, BinaryData>();

            var parser = BinaryDataParser.OfValue(pageData);

            var offsets = GetOffsetsFromPageMap(pageMap);

            for (int i = 0; i < offsets.Length - 1; i++)
            {
                var itemData =
                    parser.TakeAt(offsets[i], offsets[i + 1] - offsets[i]);

                var hid =
                    new HID(Globals.NID_TYPE_HID, i, pageIndex);

                items.Add(
                    hid,
                    itemData);
            }

            return items;
        }

        private int[] GetOffsetsFromPageMap(HNPAGEMAP pageMap)
        {
            var offsets = new List<int>();

            using (var parser = BinaryDataParser.OfValue(pageMap.AllocationTable))
            {
                for (int i = 0; i <= pageMap.AllocationCount; i++)
                {
                    offsets.Add(parser.TakeAtWithoutChangingStreamPosition(i * 2, 2, int32Decoder));
                }
            }

            return offsets.ToArray();
        }
    }
}
