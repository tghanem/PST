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

        private readonly IDataHolder<Header> cachedHeaderHolder;

        public HeaderUsageProvider(
            IDataReader dataReader,
            IDecoder<Header> headerDecoder,
            IDataHolder<Header> cachedHeaderHolder)
        {
            this.dataReader = dataReader;
            this.headerDecoder = headerDecoder;
            this.cachedHeaderHolder = cachedHeaderHolder;
        }

        public void Use(Func<Header, Header> useHeader)
        {
            cachedHeaderHolder.SetData(useHeader(GetHeader()));
        }

        public Header GetHeader()
        {
            var cachedHeader = cachedHeaderHolder.GetData();

            if (cachedHeader.HasNoValue)
            {
                var header = headerDecoder.Decode(dataReader.Read(0, 546));

                cachedHeaderHolder.SetData(header);

                return header;
            }

            return cachedHeader.Value;
        }
    }
}
