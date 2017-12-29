using System;
using System.IO;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.encodables.ndb.maps;
using pst.interfaces;
using pst.interfaces.rawallocation;
using pst.utilities;

namespace pst.impl.rawallocation
{
    class AMapBasedStreamExtender : IStreamExtender
    {
        private const int AMapExtensionSize = 496 * 8 * 64;
        private const int PMapExtensionSize = 496 * 8 * 512;

        private const long OffsetOfFirstAMap = 0x4400;
        private const long OffsetOfFirstPMap = 0x4600;

        private readonly Stream dataStream;
        private readonly IEncoder<AMap> amapEncoder;
        private readonly IEncoder<PMap> pmapEncoder;
        private readonly IEncoder<FMap> fmapEncoder;
        private readonly IEncoder<FPMap> fpmapEncoder;
        private readonly IHeaderUsageProvider headerUsageProvider;

        public AMapBasedStreamExtender(
            Stream dataStream,
            IEncoder<AMap> amapEncoder,
            IEncoder<PMap> pmapEncoder,
            IEncoder<FMap> fmapEncoder,
            IEncoder<FPMap> fpmapEncoder,
            IHeaderUsageProvider headerUsageProvider)
        {
            this.dataStream = dataStream;
            this.amapEncoder = amapEncoder;
            this.pmapEncoder = pmapEncoder;
            this.fmapEncoder = fmapEncoder;
            this.fpmapEncoder = fpmapEncoder;
            this.headerUsageProvider = headerUsageProvider;
        }

        public IB ExtendSingle()
        {
            var extensionOffset = IB.OfValue(dataStream.Length);

            var numberOfAMapsSoFar = Convert.ToInt32(extensionOffset.Subtract(OffsetOfFirstAMap) / AMapExtensionSize);
            var numberOfPMapsSoFar = Convert.ToInt32(extensionOffset.Add(0x512).Subtract(OffsetOfFirstPMap) / PMapExtensionSize);

            var createPMap = extensionOffset.Add(0x512).Subtract(OffsetOfFirstPMap) % PMapExtensionSize == 0;
            var createFMap = numberOfAMapsSoFar == 127 || numberOfAMapsSoFar == 495;
            var createFPMap = numberOfPMapsSoFar == 127 * 8 || numberOfPMapsSoFar == 495 * 8;

            var amap = CreateAMap(extensionOffset, createPMap, createFMap, createFPMap);

            var dataGenerator = BinaryDataGenerator.New();

            dataGenerator.Append(amap, amapEncoder);

            if (createPMap)
            {
                dataGenerator.Append(CreatePMap(extensionOffset + 0x200), pmapEncoder);
            }

            if (createFMap)
            {
                dataGenerator.Append(CreateFMap(extensionOffset + 0x400), fmapEncoder);
            }

            if (createFPMap)
            {
                dataGenerator.Append(CreateFPMap(extensionOffset + 0x600), fpmapEncoder);
            }

            dataGenerator.FillTo(AMapExtensionSize);

            dataGenerator.WriteTo(dataStream);

            headerUsageProvider.Use(
                header =>
                header
                .SetRoot(
                    header
                    .Root
                    .SetFileEOF(extensionOffset + AMapExtensionSize)
                    .SetLastAMapOffset(extensionOffset)
                    .SetFreeSpaceInAllAMaps(header.Root.AMapFree + AMapExtensionSize - GetDefaultAllocationsSize(createPMap, createFMap, createFPMap))));

            return extensionOffset;
        }

        private int GetDefaultAllocationsSize(bool createPMap, bool createFMap, bool createFPMap)
        {
            //the size of the AMap;
            var total = 512;

            if (createPMap) total += 512;
            if (createFMap) total += 512;
            if (createFPMap) total += 512;

            return total;
        }

        private AMap CreateAMap(long mapOffset, bool createPMap, bool createFMap, bool createFPMap)
        {
            var allocations = new byte[3];

            //the amap maps itself.
            allocations[0] = 0xFF;
            allocations[1] = (byte)(createPMap ? 0xFF : 0x00);
            allocations[2] = (byte)(createFMap ? 0xFF : 0x00);
            allocations[3] = (byte)(createFPMap ? 0xFF : 0x00);

            return CreateAMap(mapOffset, allocations);
        }

        private AMap CreateAMap(long mapOffset, params byte[] defaultAllocations)
        {
            var data = new byte[496];

            for (var i = 0; i < defaultAllocations.Length; i++)
            {
                data[i] = defaultAllocations[i];
            }

            return
                new AMap(
                    BinaryData.OfValue(data),
                    new PageTrailer(
                        Constants.ptypeAMap,
                        Constants.ptypeAMap,
                        0x0000,
                        (int)Crc32.ComputeCrc32(data),
                        BID.OfValue(mapOffset)));
        }

        private PMap CreatePMap(long mapOffset)
        {
            var data = BinaryData.OfSize(496, 0xFF);

            return
                new PMap(
                    data,
                    new PageTrailer(
                        Constants.ptypePMap,
                        Constants.ptypePMap,
                        0x0000,
                        (int)Crc32.ComputeCrc32(data.Value),
                        BID.OfValue(mapOffset)));
        }

        private FMap CreateFMap(long mapOffset)
        {
            var data = BinaryData.OfSize(496, 0xFF);

            return
                new FMap(
                    data,
                    new PageTrailer(
                        Constants.ptypeFMap,
                        Constants.ptypeFMap,
                        0x0000,
                        (int)Crc32.ComputeCrc32(data.Value),
                        BID.OfValue(mapOffset)));
        }

        private FPMap CreateFPMap(long mapOffset)
        {
            var data = BinaryData.OfSize(496, 0xFF);

            return
                new FPMap(
                    data,
                    new PageTrailer(
                        Constants.ptypeFPMap,
                        Constants.ptypeFPMap,
                        0x0000,
                        (int)Crc32.ComputeCrc32(data.Value),
                        BID.OfValue(mapOffset)));
        }
    }
}
