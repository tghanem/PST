using pst.encodables.ndb;
using pst.interfaces.ndb.allocation;
using System;

namespace pst.impl.ndb.allocation
{
    class DataAllocator : IDataAllocator
    {
        private readonly IAllocationFinder allocationFinder;
        private readonly IStreamExtender streamExtender;
        private readonly IAllocationReserver allocationReserver;

        public DataAllocator(
            IAllocationFinder allocationFinder,
            IStreamExtender streamExtender,
            IAllocationReserver allocationReserver)
        {
            this.allocationFinder = allocationFinder;
            this.streamExtender = streamExtender;
            this.allocationReserver = allocationReserver;
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
                return allocationReserver.Reserve(allocationInfo.Value);
            }

            var extensionOffset = streamExtender.ExtendSingle();

            var postExtensionAllocationInfo = allocationFinder.Find(extensionOffset, sizeOfDataInBytes);

            if (postExtensionAllocationInfo.HasNoValue)
            {
                throw new Exception("Unexpected error: could not allocate requested data size");
            }

            return allocationReserver.Reserve(postExtensionAllocationInfo.Value);
        }
    }
}
