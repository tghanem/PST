using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeReader : IHeapOnNodeReader
    {
        private readonly IHeapOnNodeItemsLoader heapOnNodeItemsLoader;
        private readonly IDataTreeReader dataTreeReader;

        public HeapOnNodeReader(
            IHeapOnNodeItemsLoader heapOnNodeItemsLoader,
            IDataTreeReader dataTreeReader)
        {
            this.heapOnNodeItemsLoader = heapOnNodeItemsLoader;
            this.dataTreeReader = dataTreeReader;
        }

        public HNHDR GetHeapOnNodeHeader(NID[] nodePath)
        {
            var externalBlock = dataTreeReader.Read(nodePath, 0);

            return HNHDR.OfValue(externalBlock[0].Take(12));
        }

        public BinaryData GetHeapItem(NID[] nodePath, HID hid)
        {
            var externalBlock = dataTreeReader.Read(nodePath, hid.BlockIndex)[0];

            int pageMapOffset;

            if (hid.BlockIndex == 0)
            {
                var hnHDR = HNHDR.OfValue(externalBlock.Take(12));

                pageMapOffset = hnHDR.PageMapOffset;
            }
            else if (hid.BlockIndex == 8 || (hid.BlockIndex - 8) % 128 == 0)
            {
                var hnBitmapHDR = HNBITMAPHDR.OfValue(externalBlock.Take(66));

                pageMapOffset = hnBitmapHDR.PageMapOffset;
            }
            else
            {
                var hnPageHDR = HNPAGEHDR.OfValue(externalBlock.Take(2));

                pageMapOffset = hnPageHDR.PageMapOffset;
            }

            var pageMap = GetPageMapFromExternalDataBlock(externalBlock, pageMapOffset);

            var items = heapOnNodeItemsLoader.Load(hid.BlockIndex, pageMap, externalBlock);

            return items[hid];
        }

        private HNPAGEMAP GetPageMapFromExternalDataBlock(BinaryData block, int pageMapOffset)
        {
            var hnPageMap = block.Take(pageMapOffset, block.Length - pageMapOffset);

            return HNPAGEMAP.OfValue(hnPageMap);
        }
    }
}
