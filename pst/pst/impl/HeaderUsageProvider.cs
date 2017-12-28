using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.io;
using System;

namespace pst.impl
{
    class HeaderUsageProvider : IHeaderUsageProvider
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;

        private Maybe<Header> cachedHeader;

        public HeaderUsageProvider(IDataReader dataReader, IDecoder<Header> headerDecoder)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
        }

        public void Use(Func<Header, Header> useHeader)
        {
            cachedHeader = Maybe<Header>.OfValue(useHeader(GetHeader()));
        }

        public Header GetHeader()
        {
            return cachedHeader.HasValue ? cachedHeader.Value : headerDecoder.Decode(dataReader.Read(0, 546));
        }
    }
}
