using pst.core;
using pst.encodables.ltp.hn;
using pst.utilities;
using System.Collections.Generic;
using System.Linq;

namespace pst.impl.ltp.hn
{
    class ExternalDataBlockForHeapOnNode
    {
        private const int MaximumRawByteCountInExternalDataBlock = 8 * 1024 - 16;

        private readonly List<BinaryData> itemsInBlock;

        public ExternalDataBlockForHeapOnNode(int blockIndex)
        {
            itemsInBlock = new List<BinaryData>();
            BlockIndex = blockIndex;
        }

        public int BlockIndex { get; }

        public int HeaderSize
        {
            get
            {
                if (BlockIndex == 0)
                {
                    return 12;
                }

                if (BlockIndex == 8 || (BlockIndex - 8) % 128 == 0)
                {
                    return 66;
                }

                return 2;
            }
        }

        public int TotalByteCountForItems => itemsInBlock.Aggregate(0, (sum, item) => sum + item.Length);

        public int NumberOfItems => itemsInBlock.Count;

        public int RawByteCountWithoutPageMap => HeaderSize + TotalByteCountForItems;

        public int RawByteCountWithoutPageMapWithPadding => RawByteCountWithoutPageMap % 2 == 0 ? RawByteCountWithoutPageMap : RawByteCountWithoutPageMap + 1;

        public int RawByteCountForPageMap => 4 + 2 * (NumberOfItems + 1);

        public int FreeSpaceSize => MaximumRawByteCountInExternalDataBlock - RawByteCountWithoutPageMapWithPadding - RawByteCountForPageMap;

        public byte FillLevel
        {
            get
            {
                if (FreeSpaceSize >= 3584) return Constants.FILL_LEVEL_EMPTY;

                if (FreeSpaceSize >= 2560 && FreeSpaceSize < 3584) return Constants.FILL_LEVEL_1;

                if (FreeSpaceSize >= 2048 && FreeSpaceSize < 2560) return Constants.FILL_LEVEL_2;

                if (FreeSpaceSize >= 1792 && FreeSpaceSize < 2048) return Constants.FILL_LEVEL_3;

                if (FreeSpaceSize >= 1536 && FreeSpaceSize < 1792) return Constants.FILL_LEVEL_4;

                if (FreeSpaceSize >= 1280 && FreeSpaceSize < 1536) return Constants.FILL_LEVEL_5;

                if (FreeSpaceSize >= 1024 && FreeSpaceSize < 1280) return Constants.FILL_LEVEL_6;

                if (FreeSpaceSize >= 768 && FreeSpaceSize < 1024) return Constants.FILL_LEVEL_7;

                if (FreeSpaceSize >= 512 && FreeSpaceSize < 768) return Constants.FILL_LEVEL_8;

                if (FreeSpaceSize >= 256 && FreeSpaceSize < 512) return Constants.FILL_LEVEL_9;

                if (FreeSpaceSize >= 128 && FreeSpaceSize < 256) return Constants.FILL_LEVEL_10;

                if (FreeSpaceSize >= 64 && FreeSpaceSize < 128) return Constants.FILL_LEVEL_11;

                if (FreeSpaceSize >= 32 && FreeSpaceSize < 64) return Constants.FILL_LEVEL_12;

                if (FreeSpaceSize >= 16 && FreeSpaceSize < 32) return Constants.FILL_LEVEL_13;

                if (FreeSpaceSize >= 8 && FreeSpaceSize < 16) return Constants.FILL_LEVEL_14;

                return Constants.FILL_LEVEL_FULL;
            }
        }

        public Maybe<int> IndexOfUserRoot { get; private set; }

        public Maybe<HID> UserRoot => IndexOfUserRoot.HasValue ? new HID(Constants.NID_TYPE_HID, IndexOfUserRoot.Value + 1, BlockIndex) : Maybe<HID>.NoValue();

        public BinaryData[] Items => itemsInBlock.ToArray();

        public HID AddItem(BinaryData value, bool isUserRoot = false)
        {
            if (isUserRoot)
            {
                IndexOfUserRoot = Maybe<int>.OfValue(itemsInBlock.Count);
            }

            var itemIndex = NumberOfItems;

            itemsInBlock.Add(value);

            return new HID(Constants.NID_TYPE_HID, itemIndex + 1, BlockIndex);
        }
    }
}