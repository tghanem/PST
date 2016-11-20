using pst.interfaces;
using pst.encodables;

namespace pst.impl
{
    class ReaderBasedDataBlockLoader<TBlockType> : IDataBlockLoader<TBlockType>
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<TBlockType> blockDecoder;
        private readonly int blockSize;

        public ReaderBasedDataBlockLoader(IDataReader dataReader, IDecoder<TBlockType> blockDecoder, int blockSize)
        {
            this.dataReader = dataReader;
            this.blockDecoder = blockDecoder;
            this.blockSize = blockSize;
        }

        public TBlockType Load(BREF blockReference)
        {
            var blockData = dataReader.Read(blockReference.ByteIndex, blockSize);

            return blockDecoder.Decode(blockData);
        }
    }
}
