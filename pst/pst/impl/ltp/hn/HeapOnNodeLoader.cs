using pst.interfaces.ltp.hn;
using System.Collections.Generic;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using pst.interfaces;
using pst.encodables.ltp.hn;
using pst.encodables.ndb.blocks.data;
using pst.utilities;

namespace pst.impl.ltp.hn
{
    class HeapOnNodeLoader
        : IHeapOnNodeLoader
    {
        private readonly IDecoder<HNHDR> hnHDRDecoder;
        private readonly IDecoder<HNPAGEHDR> hnPageHDRDecoder;
        private readonly IDecoder<HNPAGEMAP> hnPageMapDecoder;
        private readonly IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder;
        private readonly IHeapOnNodeItemsLoader heapOnNodeItemsLoader;
        private readonly IDataTreeLeafNodesEnumerator externalDataBlocksLoader;

        public HeapOnNodeLoader(
            IDecoder<HNHDR> hnHDRDecoder,
            IDecoder<HNPAGEHDR> hnPageHDRDecoder,
            IDecoder<HNPAGEMAP> hnPageMapDecoder,
            IDecoder<HNBITMAPHDR> hnBitmapHDRDecoder,
            IHeapOnNodeItemsLoader heapOnNodeItemsLoader,
            IDataTreeLeafNodesEnumerator externalDataBlocksLoader)
        {
            this.hnHDRDecoder = hnHDRDecoder;
            this.hnPageHDRDecoder = hnPageHDRDecoder;
            this.hnPageMapDecoder = hnPageMapDecoder;
            this.hnBitmapHDRDecoder = hnBitmapHDRDecoder;
            this.heapOnNodeItemsLoader = heapOnNodeItemsLoader;
            this.externalDataBlocksLoader = externalDataBlocksLoader;
        }

        public HeapOnNode Load(
            IDataBlockReader<LBBTEntry> reader,
            IReadOnlyDictionary<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry)
        {
            var externalDataBlocks =
                externalDataBlocksLoader
                .Enumerate(
                    reader,
                    blockIdToEntryMapping,
                    blockEntry);

            if (externalDataBlocks.Length == 0)
                return new HeapOnNode(HID.Zero);

            var hnHDR =
                hnHDRDecoder.Decode(externalDataBlocks[0].Data.Take(12));

            var heapOnNode =
                new HeapOnNode(hnHDR.UserRoot);

            heapOnNode.FillFrom(
                heapOnNodeItemsLoader.Load(
                    0,
                    GetPageMapFromExternalDataBlock(
                        externalDataBlocks[0],
                        hnHDR.PageMapOffset),
                    externalDataBlocks[0].Data));

            for(var i = 0; i < externalDataBlocks.Length; i++)
            {
                var pageMapOffset = 0;

                var parser = BinaryDataParser.OfValue(externalDataBlocks[i].Data);

                if (i == 8 || (i - 8) % 128 == 0)
                {
                    var hnBitmapHDR =
                        parser
                        .TakeAndSkip(66, hnBitmapHDRDecoder);

                    pageMapOffset = hnBitmapHDR.PageMapOffset;
                }
                else
                {
                    var hnPageHDR =
                        parser.TakeAndSkip(2, hnPageHDRDecoder);

                    pageMapOffset = hnPageHDR.PageMapOffset;
                }

                heapOnNode.FillFrom(
                    heapOnNodeItemsLoader.Load(
                        i,
                        GetPageMapFromExternalDataBlock(
                            externalDataBlocks[i],
                            pageMapOffset),
                        externalDataBlocks[i].Data));
            }

            return heapOnNode;
        }

        private HNPAGEMAP GetPageMapFromExternalDataBlock(ExternalDataBlock block, int pageMapOffset)
        {
            var parser = BinaryDataParser.OfValue(block.Data);

            return
                parser
                .TakeAtWithoutChangingStreamPosition(
                    pageMapOffset,
                    block.Data.Length - pageMapOffset,
                    hnPageMapDecoder);
        }
    }
}
