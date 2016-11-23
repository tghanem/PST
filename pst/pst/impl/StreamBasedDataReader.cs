using pst.interfaces;
using pst.encodables.ndb;
using pst.utilities;
using System.IO;

namespace pst.impl
{
    class StreamBasedDataReader : IDataReader
    {
        private readonly Stream dataStream;

        public StreamBasedDataReader(Stream dataStream)
        {
            this.dataStream = dataStream;
        }

        public BinaryData Read(IB byteIndex, int length)
        {
            var buffer = new byte[length];

            var position = dataStream.Position;

            dataStream.Seek(byteIndex.Value, SeekOrigin.Begin);

            dataStream.Read(buffer, 0, length);

            dataStream.Seek(position, SeekOrigin.Begin);

            return BinaryData.OfValue(buffer);
        }
    }
}
