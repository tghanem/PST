using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.encodables.ndb.maps;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.rawallocation;
using pst.utilities;

namespace pst.impl.rawallocation
{
    class AMapBasedAllocationReserver : IAllocationReserver
    {
        private readonly IRegionUpdater<AMap> amapRegionUpdater;
        private readonly IHeaderUsageProvider headerUsageProvider;

        public AMapBasedAllocationReserver(IRegionUpdater<AMap> amapRegionUpdater, IHeaderUsageProvider headerUsageProvider)
        {
            this.amapRegionUpdater = amapRegionUpdater;
            this.headerUsageProvider = headerUsageProvider;
        }

        public IB Reserve(AllocationInfo allocationInfo)
        {
            amapRegionUpdater.Update(
                allocationInfo.MapOffset,
                map =>
                {
                    var bits = map.Data.Value.ToBits();

                    for (var i = allocationInfo.BitStartIndex; i <= allocationInfo.BitEndIndex; i++)
                    {
                        bits[i] = 1;
                    }

                    var bytes = bits.ToBytes();

                    return
                        new AMap(
                            BinaryData.OfValue(bytes),
                            new PageTrailer(
                                Constants.ptypeAMap,
                                Constants.ptypeAMap,
                                0x0000,
                                Crc32.ComputeCrc32(bytes),
                                BID.OfValue(allocationInfo.MapOffset)));
                });

            var allocateSize = (allocationInfo.BitEndIndex - allocationInfo.BitStartIndex + 1) * 64;

            headerUsageProvider.Use(header => header.SetRoot(header.Root.SetFreeSpaceInAllAMaps(header.Root.AMapFree - allocateSize)));

            return IB.OfValue(allocationInfo.MapOffset + allocationInfo.BitStartIndex * 64);
        }
    }
}
