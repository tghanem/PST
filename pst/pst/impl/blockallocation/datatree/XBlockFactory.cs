using pst.encodables.ndb;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.utilities;

namespace pst.impl.blockallocation.datatree
{
    class XBlockFactory : IDataBlockFactory<BlockIdsWithTotalNumberOfBytesInReferencedBlocks, InternalDataBlock>
    {
        private readonly IEncoder<BID> bidEncoder;
        private readonly int blockLevel;

        public XBlockFactory(IEncoder<BID> bidEncoder, int blockLevel)
        {
            this.bidEncoder = bidEncoder;
            this.blockLevel = blockLevel;
        }

        public InternalDataBlock Create(
            IB blockOffset,
            BID blockId,
            BlockIdsWithTotalNumberOfBytesInReferencedBlocks data)
        {
            var encodedExternalBlockIds = data.BlockIds.Encode(bidEncoder);

            return
                new InternalDataBlock(
                    0x01,
                    blockLevel,
                    data.BlockIds.Length,
                    data.TotalNumberOfBytesInReferencedBlocks,
                    encodedExternalBlockIds,
                    BinaryData.OfSize(Utilities.GetInternalDataBlockPaddingSize(encodedExternalBlockIds.Length)),
                    new BlockTrailer(
                        encodedExternalBlockIds.Length + 8,
                        BlockSignature.Calculate(blockOffset, blockId),
                        Crc32.ComputeCrc32(GetDataToCalculateCrc32(encodedExternalBlockIds, data.BlockIds.Length, data.TotalNumberOfBytesInReferencedBlocks)),
                        blockId));
        }

        private BinaryData GetDataToCalculateCrc32(
            BinaryData encodedExternalBlockIds,
            int numberOfExternalBlockIds,
            int totalNumberOfBytesInReferencedBlocks)
        {
            return
                BinaryDataGenerator.New()
                .Append((byte)0x01)
                .Append((byte)blockLevel)
                .Append((short)numberOfExternalBlockIds)
                .Append(totalNumberOfBytesInReferencedBlocks)
                .Append(encodedExternalBlockIds)
                .GetData();
        }
    }
}
