using pst.encodables.ndb;
using pst.interfaces.ndb.allocation;
using System;

namespace pst.impl.ndb.allocation
{
    class DataAllocator : IDataAllocator
    {
        private readonly IAllocationFinder allocationFinder;
        private readonly IStreamExtender amapBasedStreamExtender;
        private readonly IAMapAllocationReserver amapAllocationReserver;

        public DataAllocator(
            IAllocationFinder allocationFinder,
            IStreamExtender amapBasedStreamExtender,
            IAMapAllocationReserver amapAllocationReserver)
        {
            this.allocationFinder = allocationFinder;
            this.amapBasedStreamExtender = amapBasedStreamExtender;
            this.amapAllocationReserver = amapAllocationReserver;
        }

        public IB Allocate(int sizeOfDataInBytes)
        {
            if (sizeOfDataInBytes > 8 * 1024)
            {
                throw new Exception($"BUG: data size to allocate {sizeOfDataInBytes} bytes is larger than 8K");
            }

            var allocationInfo = allocationFinder.Find(sizeOfDataInBytes);

            if (allocationInfo.HasValue)
            {
                return amapAllocationReserver.Reserve(allocationInfo.Value);
            }

            var extensionOffset = amapBasedStreamExtender.ExtendSingle();

            var postExtensionAllocationInfo = allocationFinder.Find(extensionOffset, sizeOfDataInBytes);

            if (postExtensionAllocationInfo.HasNoValue)
            {
                throw new Exception("Unexpected error: could not allocate requested data size");
            }

            return amapAllocationReserver.Reserve(postExtensionAllocationInfo.Value);
        }
    }
}
