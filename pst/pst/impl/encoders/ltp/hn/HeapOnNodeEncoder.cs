using pst.encodables.ltp.hn;
using pst.impl.ltp.hn;
using pst.interfaces;
using pst.interfaces.ltp.hn;
using pst.utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pst.impl.encoders.ltp.hn
{
    class HeapOnNodeEncoder : IHeapOnNodeEncoder
    {
        private readonly IEncoder<HNHDR> headerEncoder;
        private readonly IEncoder<HNBITMAPHDR> bitmapHeaderEncoder;
        private readonly IEncoder<HNPAGEHDR> pageHeaderEncoder;
        private readonly IEncoder<HNPAGEMAP> pageMapEncoder;

        public HeapOnNodeEncoder(
            IEncoder<HNHDR> headerEncoder,
            IEncoder<HNBITMAPHDR> bitmapHeaderEncoder,
            IEncoder<HNPAGEHDR> pageHeaderEncoder,
            IEncoder<HNPAGEMAP> pageMapEncoder)
        {
            this.headerEncoder = headerEncoder;
            this.bitmapHeaderEncoder = bitmapHeaderEncoder;
            this.pageHeaderEncoder = pageHeaderEncoder;
            this.pageMapEncoder = pageMapEncoder;
        }

        public BinaryData[] Encode(ExternalDataBlockForHeapOnNode[] blocks, int clientSignature)
        {
            var encodedBlocks = new List<BinaryData>();

            for (var i = 0; i < blocks.Length; i++)
            {
                if (i == 0)
                {
                    var blockWithUserRoot = blocks.First(b => b.IndexOfUserRoot.HasValue);

                    var fillLevelsForTheFirst8Blocks = GetFillLevels(blocks, 0, 8);

                    var encodedFirstBlock = EncodeFirstBlock(blocks[i], blockWithUserRoot.UserRoot.Value, fillLevelsForTheFirst8Blocks, clientSignature);

                    encodedBlocks.Add(encodedFirstBlock);
                }
                else if (i == 8 || (i - 8) % 128 == 0)
                {
                    var fillLevelsForTheNext128Blocks = GetFillLevels(blocks, i, i + 128);

                    var encodedBlockWithNewFillLevel = EncodeBlockWithNewFillLevel(blocks[i], fillLevelsForTheNext128Blocks);

                    encodedBlocks.Add(encodedBlockWithNewFillLevel);
                }
                else
                {
                    encodedBlocks.Add(EncodeBlock(blocks[i]));
                }
            }

            return encodedBlocks.ToArray();
        }

        private BinaryData EncodeFirstBlock(
            ExternalDataBlockForHeapOnNode block,
            HID userRoot,
            BinaryData fillLevelsForTheFirst8Blocks,
            int clientSignature)
        {
            var hnHDR =
                new HNHDR(
                    block.RawByteCountWithoutPageMapWithPadding,
                    0xEC,
                    clientSignature,
                    userRoot,
                    fillLevelsForTheFirst8Blocks);

            return EncodeBlock(block, headerEncoder.Encode(hnHDR));
        }

        private BinaryData EncodeBlockWithNewFillLevel(
            ExternalDataBlockForHeapOnNode block,
            BinaryData fillLevelsForTheNext128Blocks)
        {
            var hnBitmapHeader =
                new HNBITMAPHDR(
                    block.RawByteCountWithoutPageMapWithPadding,
                    fillLevelsForTheNext128Blocks);

            return EncodeBlock(block, bitmapHeaderEncoder.Encode(hnBitmapHeader));
        }

        private BinaryData EncodeBlock(
            ExternalDataBlockForHeapOnNode block)
        {
            var hnPageHeader = new HNPAGEHDR(block.RawByteCountWithoutPageMapWithPadding);


            return EncodeBlock(block, pageHeaderEncoder.Encode(hnPageHeader));
        }

        private BinaryData EncodeBlock(
            ExternalDataBlockForHeapOnNode block,
            BinaryData encodedHeader)
        {
            var pageMap =
                new HNPAGEMAP(
                    block.NumberOfItems + 1,
                    0,
                    CreateAllocationTable(block.Items, block.HeaderSize));

            return
                BinaryDataGenerator
                .New()
                .Append(encodedHeader)
                .Append(block.Items)
                .FillTo(block.RawByteCountWithoutPageMapWithPadding)
                .Append(pageMap, pageMapEncoder)
                .GetData();
        }

        private BinaryData CreateAllocationTable(
            BinaryData[] items,
            int sizeOfTheHeader)
        {
            var generator = BinaryDataGenerator.New();

            var itemOffset = sizeOfTheHeader;

            for (var i = 0; i < items.Length + 1; i++)
            {
                generator = generator.Append((short)itemOffset);
                itemOffset = itemOffset + items[i].Length;
            }

            return generator.GetData();
        }

        private BinaryData GetFillLevels(
            ExternalDataBlockForHeapOnNode[] blocks,
            int startIndex,
            int endIndex)
        {
            var stream = new MemoryStream();

            for (var i = startIndex; i < endIndex; i += 2)
            {
                var value = blocks[i].FillLevel & blocks[i + 1].FillLevel << 4;
                stream.WriteByte((byte)value);
            }

            return BinaryData.OfValue(stream.ToArray());
        }
    }
}
