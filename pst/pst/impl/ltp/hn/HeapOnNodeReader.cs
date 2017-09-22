using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeReader : IHeapOnNodeReader
    {
        private readonly IDecoder<HNHDR> hnHDRDecoder;
        private readonly IDecoder<HNPAGEHDR> hnPageHDRDecoder;
        private readonly IDecoder<HNPAGEMAP> hnPageMapDecoder;
        private readonly IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder;
        private readonly IHeapOnNodeItemsLoader heapOnNodeItemsLoader;
        private readonly IExternalDataBlockReader externalDataBlockReader;

        public HeapOnNodeReader(
            IDecoder<HNHDR> hnHDRDecoder,
            IDecoder<HNPAGEHDR> hnPageHDRDecoder,
            IDecoder<HNPAGEMAP> hnPageMapDecoder,
            IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder,
            IHeapOnNodeItemsLoader heapOnNodeItemsLoader,
            IExternalDataBlockReader externalDataBlockReader)
        {
            this.hnHDRDecoder = hnHDRDecoder;
            this.hnPageHDRDecoder = hnPageHDRDecoder;
            this.hnPageMapDecoder = hnPageMapDecoder;
            this.hnBitmapHDRDecoder = hnBitmapHDRDecoder;
            this.heapOnNodeItemsLoader = heapOnNodeItemsLoader;
            this.externalDataBlockReader = externalDataBlockReader;
        }

        public HNHDR GetHeapOnNodeHeader(NodePath nodePath)
        {
            var externalBlock = externalDataBlockReader.Read(nodePath, 0);

            return hnHDRDecoder.Decode(externalBlock.Take(12));
        }

        public BinaryData GetHeapItem(NodePath nodePath, HID hid)
        {
            var externalBlock = externalDataBlockReader.Read(nodePath, hid.BlockIndex);

            int pageMapOffset;

            if (hid.BlockIndex == 0)
            {
                var hnHDR = hnHDRDecoder.Decode(externalBlock.Take(12));

                pageMapOffset = hnHDR.PageMapOffset;
            }
            else if (hid.BlockIndex == 8 || (hid.BlockIndex - 8) % 128 == 0)
            {
                var hnBitmapHDR = hnBitmapHDRDecoder.Decode(externalBlock.Take(66));

                pageMapOffset = hnBitmapHDR.PageMapOffset;
            }
            else
            {
                var hnPageHDR = hnPageHDRDecoder.Decode(externalBlock.Take(2));

                pageMapOffset = hnPageHDR.PageMapOffset;
            }

            var pageMap = GetPageMapFromExternalDataBlock(externalBlock, pageMapOffset);

            var items = heapOnNodeItemsLoader.Load(hid.BlockIndex, pageMap, externalBlock);

            return items[hid];
        }

        private HNPAGEMAP GetPageMapFromExternalDataBlock(BinaryData block, int pageMapOffset)
        {
            var hnPageMap = block.Take(pageMapOffset, block.Length - pageMapOffset);

            return hnPageMapDecoder.Decode(hnPageMap);
        }
    }
}
