using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
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
        private readonly IDataTreeLeafNodesEnumerator externalDataBlockIdsLoader;

        private readonly IMapper<BID, LBBTEntry> blockIdToEntryMapping;
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        public HeapOnNodeReader(
            IDecoder<HNHDR> hnHDRDecoder,
            IDecoder<HNPAGEHDR> hnPageHDRDecoder,
            IDecoder<HNPAGEMAP> hnPageMapDecoder,
            IDecoder<BinaryData> blockDataDecoder,
            IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder,
            IHeapOnNodeItemsLoader heapOnNodeItemsLoader,
            IDataTreeLeafNodesEnumerator externalDataBlockIdsLoader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.hnHDRDecoder = hnHDRDecoder;
            this.hnPageHDRDecoder = hnPageHDRDecoder;
            this.hnPageMapDecoder = hnPageMapDecoder;
            this.blockDataDecoder = blockDataDecoder;
            this.hnBitmapHDRDecoder = hnBitmapHDRDecoder;
            this.heapOnNodeItemsLoader = heapOnNodeItemsLoader;
            this.externalDataBlockIdsLoader = externalDataBlockIdsLoader;
            this.blockIdToEntryMapping = blockIdToEntryMapping;
            this.dataBlockReader = dataBlockReader;
        }

        public HNHDR GetHeapOnNodeHeader(LBBTEntry blockEntry)
        {
            var externalBlock =
                ReadExternalDataBlock(blockEntry, 0);

            return hnHDRDecoder.Decode(externalBlock.Take(12));
        }

        public BinaryData GetHeapItem(LBBTEntry blockEntry, HID hid)
        {
            var externalBlock =
                ReadExternalDataBlock(blockEntry, hid.BlockIndex);

            var parser = BinaryDataParser.OfValue(externalBlock);

            var pageMapOffset = 0;

            if (hid.BlockIndex == 0)
            {
                var hnHDR = hnHDRDecoder.Decode(externalBlock.Take(12));

                pageMapOffset = hnHDR.PageMapOffset;
            }
            else if (hid.BlockIndex == 8 || (hid.BlockIndex - 8) % 128 == 0)
            {
                var hnBitmapHDR = parser .TakeAndSkip(66, hnBitmapHDRDecoder);

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

        private BinaryData ReadExternalDataBlock(LBBTEntry blockEntry, int blockIndex)
        {
            var externalDataBlockIds =
                externalDataBlockIdsLoader.Enumerate(blockEntry);

            var externalBlockLbbtEntry =
                blockIdToEntryMapping.Map(externalDataBlockIds[blockIndex]);

            var externalDataBlock =
                dataBlockReader.Read(externalBlockLbbtEntry, externalBlockLbbtEntry.GetBlockSize());

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
