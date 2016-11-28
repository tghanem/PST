using pst.interfaces.io;
using pst.encodables.ndb;
using pst.utilities;
using System.IO;

namespace pst.impl.io
{
    class StreamBasedDataReader : IDataReader
    {
        private readonly Stream stream;

        public StreamBasedDataReader(Stream stream)
        {
            this.stream = stream;
        }

        public BinaryData Read(IB byteIndex, int count)
        {
            stream.Seek(byteIndex.Value, SeekOrigin.Begin);

            var buffer = new byte[count];

            stream.Read(buffer, 0, count);

            return BinaryData.OfValue(buffer);
        }
    }
}
