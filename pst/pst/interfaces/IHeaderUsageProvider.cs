using System;
using pst.encodables.ndb;

namespace pst.interfaces
{
    interface IHeaderReader
    {
        Header GetHeader();
    }

    interface IHeaderUsageProvider : IHeaderReader
    {
        void Use(Func<Header, Header> useHeader);
    }
}