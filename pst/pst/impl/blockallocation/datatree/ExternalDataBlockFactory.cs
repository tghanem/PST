using pst.encodables.ndb;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.utilities;

namespace pst.impl.blockallocation.datatree
{
    class ExternalDataBlockFactory : IDataBlockFactory<BinaryData, ExternalDataBlock>
    {
        private readonly IBlockDataObfuscator dataObfuscator;

        public ExternalDataBlockFactory(IBlockDataObfuscator dataObfuscator)
        {
            this.dataObfuscator = dataObfuscator;
        }

        public ExternalDataBlock Create(IB blockOffset, BID blockId, BinaryData data)
        {
            var obfuscatedData = dataObfuscator.Obfuscate(data, blockId);

            return
                new ExternalDataBlock(
                    obfuscatedData,
                    BinaryData.OfSize(Utilities.GetExternalDataBlockPaddingSize(obfuscatedData.Length)),
                    new BlockTrailer(
                        obfuscatedData.Length,
                        BlockSignature.Calculate(blockOffset, blockId),
                        Crc32.ComputeCrc32(obfuscatedData),
                        blockId));
        }
    }
}
