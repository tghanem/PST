using pst.encodables.ndb;
using pst.interfaces.io;
using pst.utilities;
using System.IO;

namespace pst.impl.io
{
    class StreamBasedBlockReader : IDataBlockReader<BREF>
    {
        private readonly Stream stream;

        public StreamBasedBlockReader(Stream stream)
        {
            this.stream = stream;
        }

        public BinaryData Read(BREF blockReference, int blockSize)
        {
            var buffer = new byte[blockSize];

            stream.Seek(blockReference.ByteIndex.Value, SeekOrigin.Begin);

            stream.Read(buffer, 0, blockSize);

            return BinaryData.OfValue(buffer);
        }
    }
}
