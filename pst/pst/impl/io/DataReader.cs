using System.IO;
using pst.interfaces.io;
using pst.utilities;

namespace pst.impl.io
{
    class DataReader : IDataReader
    {
        private readonly Stream stream;

        public DataReader(Stream stream)
        {
            this.stream = stream;
        }

        public BinaryData Read(long offset, int count)
        {
            var buffer = new byte[count];

            stream.Seek(offset, SeekOrigin.Begin);

            stream.Read(buffer, 0, count);

            return BinaryData.OfValue(buffer);
        }
    }
}
