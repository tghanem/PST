using pst.interfaces.ndb;
using System;
using pst.encodables.ndb.blocks.data;
using pst.interfaces.io;
using pst.interfaces;
using pst.encodables.ndb;

namespace pst.impl.ndb
{
    class SingleExternalDataBlockBasedOrderedDataBlockCollection : IOrderedDataBlockCollection
    {
        private readonly IDataReader dataReader;

        private readonly IDecoder<ExternalDataBlock> blockDecoder;

        private readonly IB blockByteIndex;

        private readonly int blockSize;

        public SingleExternalDataBlockBasedOrderedDataBlockCollection(IDataReader dataReader, IDecoder<ExternalDataBlock> blockDecoder, IB blockByteIndex, int blockSize)
        {
            this.dataReader = dataReader;
            this.blockDecoder = blockDecoder;
            this.blockByteIndex = blockByteIndex;
            this.blockSize = blockSize;
        }

        public ExternalDataBlock GetDataBlockAt(int blockIndex)
        {
            if (blockIndex < 0)
                throw new Exception("Invalid block index");

            if (blockIndex > 0)
                throw new IndexOutOfRangeException();

            var encodedBlock = dataReader.Read(blockByteIndex, blockSize);

            return blockDecoder.Decode(encodedBlock);
        }
    }
}
