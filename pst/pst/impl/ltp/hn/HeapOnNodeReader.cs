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
        private readonly IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder;
        private readonly IBlockDataDeObfuscator blockDataDeObfuscator;
        private readonly IHeapOnNodeItemsLoader heapOnNodeItemsLoader;
        private readonly IDataBlockEntryFinder dataBlockEntryFinder;

        private readonly IDataBlockReader dataBlockReader;

        public HeapOnNodeReader(
            IDecoder<HNHDR> hnHDRDecoder,
            IDecoder<HNPAGEHDR> hnPageHDRDecoder,
            IDecoder<HNPAGEMAP> hnPageMapDecoder,
            IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder,
            IBlockDataDeObfuscator blockDataDeObfuscator,
            IHeapOnNodeItemsLoader heapOnNodeItemsLoader,
            IDataBlockEntryFinder dataBlockEntryFinder,
            IDataBlockReader dataBlockReader)
        {
            this.hnHDRDecoder = hnHDRDecoder;
            this.hnPageHDRDecoder = hnPageHDRDecoder;
            this.hnPageMapDecoder = hnPageMapDecoder;
            this.hnBitmapHDRDecoder = hnBitmapHDRDecoder;
            this.blockDataDeObfuscator = blockDataDeObfuscator;
            this.heapOnNodeItemsLoader = heapOnNodeItemsLoader;
            this.dataBlockEntryFinder = dataBlockEntryFinder;

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

            var items =
                heapOnNodeItemsLoader
                .Load(hid.BlockIndex, pageMap, externalBlock);

            return items[hid];
        }

        private BinaryData ReadExternalDataBlock(BID blockId, int blockIndex)
        {
            var dataBlockTree = dataBlockEntryFinder.Find(blockId);

            var actualBlockId = blockId;

            if (dataBlockTree.Value.ChildBlockIds.HasValueAnd(childBlockIds => childBlockIds.Length > 0))
            {
                actualBlockId = dataBlockTree.Value.ChildBlockIds.Value[blockIndex];
            }

            var externalDataBlock = dataBlockReader.Read(actualBlockId);

            return blockDataDeObfuscator.DeObfuscate(externalDataBlock, actualBlockId);
        }

        private HNPAGEMAP GetPageMapFromExternalDataBlock(BinaryData block, int pageMapOffset)
        {
            var hnPageMap = block.Take(pageMapOffset, block.Length - pageMapOffset);

            return hnPageMapDecoder.Decode(hnPageMap);
        }
    }
}
