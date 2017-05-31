using pst.interfaces;
using pst.interfaces.io;
using pst.utilities;

namespace pst.impl.io
{
    class BlockReaderThatDecodesTheBlockData<TBlockReference>
        : IDataBlockReader<TBlockReference>

        where TBlockReference : class
    {
        private readonly IDataBlockReader<TBlockReference> actualReader;
        private readonly IDecoder<BinaryData> blockDataDecoder;

        public BlockReaderThatDecodesTheBlockData(
            IDataBlockReader<TBlockReference> actualReader,
            IDecoder<BinaryData> blockDataDecoder)
        {
            this.actualReader = actualReader;
            this.blockDataDecoder = blockDataDecoder;
        }

        public BinaryData Read(TBlockReference blockReference, int blockSize)
        {
            var actualBlock = actualReader.Read(blockReference, blockSize);

            return blockDataDecoder.Decode(actualBlock);
        }
    }
}
