using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.impl.ndb
{
    class HeaderBasedNIDAllocator : INIDAllocator
    {
        private readonly IHeaderUsageProvider headerUsageProvider;

        public HeaderBasedNIDAllocator(IHeaderUsageProvider headerUsageProvider)
        {
            this.headerUsageProvider = headerUsageProvider;
        }

        public NID Allocate(int type)
        {
            NID nid = NID.Zero;

            headerUsageProvider.Use(
                header =>
                {
                    nid = header.NIDs[type];

                    return header.IncrementNIDForType(type);
                });

            return nid;
        }
    }
}
