using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;

namespace pst.impl
{
    class HeaderReader : IHeaderReader
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;

        public HeaderReader(IDataReader dataReader, IDecoder<Header> headerDecoder)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
        }

        public Header GetHeader()
        {
            return headerDecoder.Decode(dataReader.Read(0, 546));
        }
    }
}
