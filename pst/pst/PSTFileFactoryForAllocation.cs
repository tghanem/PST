using pst.encodables.ndb.maps;
using pst.impl;
using pst.impl.decoders.ndb;
using pst.impl.encoders.ndb;
using pst.impl.encoders.ndb.btree;
using pst.impl.encoders.ndb.maps;
using pst.impl.encoders.primitives;
using pst.impl.io;
using System.IO;
using pst.impl.rawallocation;
using pst.interfaces.rawallocation;

namespace pst
{
    public partial class PSTFile
    {
        private static IDataAllocator CreateDataAllocator(
            Stream dataStream)
        {
            return
                new DataAllocator(
                    CreateAllocationFinder(dataStream),
                    CreateAMapBasedStreamExtender(dataStream),
                    CreateAMapBasedAllocationReserver(dataStream));
        }

        private static IAllocationReserver CreateAMapBasedAllocationReserver(Stream dataStream)
        {
            return
                new AMapBasedAllocationReserver(
                    new StreamBasedRegionUpdater<AMap>(
                        dataStream,
                        CreateMapEncoder(),
                        new AMapDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder())),
                        0x200),
                    CreateHeaderUsageProvider(dataStream));
        }

        private static IStreamExtender CreateAMapBasedStreamExtender(
            Stream dataStream)
        {
            return
                new AMapBasedStreamExtender(
                    dataStream,
                    CreateMapEncoder(),
                    CreateMapEncoder(),
                    CreateMapEncoder(),
                    CreateMapEncoder(),
                    new HeaderUsageProvider(
                        new DataReader(dataStream),
                        CreateHeaderDecoder()));
        }

        private static AMapBasedAllocationFinder CreateAllocationFinder(
            Stream dataStream)
        {
            return
                new AMapBasedAllocationFinder(
                    new DataReader(dataStream),
                    new AMapDecoder(
                        new PageTrailerDecoder(
                            new BIDDecoder())));
        }

        private static MapEncoder CreateMapEncoder()
        {
            return
                new MapEncoder(
                    new PageTrailerEncoder(
                        new Int32Encoder(),
                        new BIDEncoder()));
        }
    }
}
