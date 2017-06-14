using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;
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
        private readonly IDecoder<BinaryData> blockDataDecoder;
        private readonly IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder;
        private readonly IHeapOnNodeItemsLoader heapOnNodeItemsLoader;
        private readonly IDataTreeLeafBIDsEnumerator externalDataBlockIdsLoader;

        private readonly IDataBlockReader dataBlockReader;

        public HeapOnNodeReader(
            IDecoder<HNHDR> hnHDRDecoder,
            IDecoder<HNPAGEHDR> hnPageHDRDecoder,
            IDecoder<HNPAGEMAP> hnPageMapDecoder,
            IDecoder<BinaryData> blockDataDecoder,
            IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder,
            IHeapOnNodeItemsLoader heapOnNodeItemsLoader,
            IDataTreeLeafBIDsEnumerator externalDataBlockIdsLoader,
            IDataBlockReader dataBlockReader)
        {
            this.hnHDRDecoder = hnHDRDecoder;
            this.hnPageHDRDecoder = hnPageHDRDecoder;
            this.hnPageMapDecoder = hnPageMapDecoder;
            this.blockDataDecoder = blockDataDecoder;
            this.hnBitmapHDRDecoder = hnBitmapHDRDecoder;
            this.heapOnNodeItemsLoader = heapOnNodeItemsLoader;
            this.externalDataBlockIdsLoader = externalDataBlockIdsLoader;
            this.dataBlockReader = dataBlockReader;
        }

        public HNHDR GetHeapOnNodeHeader(BID blockId)
        {
            var externalBlock =
                ReadExternalDataBlock(blockId, 0);

            return hnHDRDecoder.Decode(externalBlock.Take(12));
        }

        public BinaryData GetHeapItem(BID blockId, HID hid)
        {
            var externalBlock =
                ReadExternalDataBlock(blockId, hid.BlockIndex);

            var parser = BinaryDataParser.OfValue(externalBlock);

            int pageMapOffset;

            if (hid.BlockIndex == 0)
            {
                var hnHDR = hnHDRDecoder.Decode(externalBlock.Take(12));

                pageMapOffset = hnHDR.PageMapOffset;
            }
            else if (hid.BlockIndex == 8 || (hid.BlockIndex - 8) % 128 == 0)
            {
                var hnBitmapHDR = parser.TakeAndSkip(66, hnBitmapHDRDecoder);

                pageMapOffset = hnBitmapHDR.PageMapOffset;
            }
            else
            {
                var hnPageHDR = parser.TakeAndSkip(2, hnPageHDRDecoder);

                pageMapOffset = hnPageHDR.PageMapOffset;
            }

            var pageMap = GetPageMapFromExternalDataBlock(externalBlock, pageMapOffset);

            var items =
                heapOnNodeItemsLoader
                .Load(hid.BlockIndex, pageMap, externalBlock);

            return items[hid];
        }

        private BinaryData ReadExternalDataBlock(BID blockId, int blockIndex)
        {
            var externalDataBlockIds =
                externalDataBlockIdsLoader.Enumerate(blockId);

            var externalDataBlock =
                dataBlockReader.Read(externalDataBlockIds[blockIndex]);

            return blockDataDecoder.Decode(externalDataBlock);
        }

        private HNPAGEMAP GetPageMapFromExternalDataBlock(BinaryData block, int pageMapOffset)
        {
            var parser = BinaryDataParser.OfValue(block);

            return
                parser
                .TakeAtWithoutChangingStreamPosition(pageMapOffset, block.Length - pageMapOffset, hnPageMapDecoder);
        }
    }
}
