using pst.encodables.ndb;
using pst.interfaces.io;
using pst.utilities;

namespace pst.impl.io
{
    class BlockReferenceBasedDataBlockReader : IDataBlockReader<BREF>
    {
        private readonly IDataReader dataReader;

        public BlockReferenceBasedDataBlockReader(IDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        public BinaryData Read(BREF blockId, int blockSize)
        {
            return dataReader.Read(blockId.ByteIndex.Value, blockSize);
        }
    }
}
