using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.encodables.ndb.maps;
using pst.impl;
using pst.impl.blockallocation.datatree;
using pst.impl.decoders.ndb;
using pst.impl.encoders.ltp.hn;
using pst.impl.encoders.ndb;
using pst.impl.encoders.ndb.blocks;
using pst.impl.encoders.ndb.btree;
using pst.impl.encoders.ndb.maps;
using pst.impl.io;
using pst.impl.ltp.hn;
using pst.impl.rawallocation;
using pst.interfaces;
using pst.interfaces.blockallocation.datatree;
using pst.interfaces.ltp.hn;
using pst.interfaces.rawallocation;
using pst.utilities;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IFactory<IHeapOnNodeGenerator> CreateHeapOnNodeGeneratorFactory(
            Stream dataStream,
            List<LBBTEntry> allocatedBlockBTreeEntries,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new FuncBasedFactory<IHeapOnNodeGenerator>(
                    () =>
                    new HeapOnNodeGenerator(
                        new HeapOnNodeEncoder(
                            new HNHDREncoder(),
                            new HNBITMAPHDREncoder(),
                            new HNPAGEHDREncoder(),
                            new HNPAGEMAPEncoder()),
                        CreateDataTreeAllocator(dataStream, allocatedBlockBTreeEntries, cachedHeaderHolder)));
        }

        private static IDataTreeAllocator CreateDataTreeAllocator(
            Stream dataStream,
            List<LBBTEntry> allocatedBlockBTreeEntries,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new DataTreeAllocator(
                    CreateExternalDataBlockAllocator(dataStream, allocatedBlockBTreeEntries, cachedHeaderHolder),
                    CreateInternalDataBlockAllocator(dataStream, allocatedBlockBTreeEntries, cachedHeaderHolder, internalBlockLevel: 0x01),
                    CreateInternalDataBlockAllocator(dataStream, allocatedBlockBTreeEntries, cachedHeaderHolder, internalBlockLevel: 0x02));
        }

        private static IDataBlockAllocator<BlockIdsWithTotalNumberOfBytesInReferencedBlocks> CreateInternalDataBlockAllocator(
            Stream dataStream,
            List<LBBTEntry> allocatedBlockBTreeEntries,
            IDataHolder<Header> cachedHeaderHolder,
            int internalBlockLevel)
        {
            return
                new DataBlockAllocator<InternalDataBlock, BlockIdsWithTotalNumberOfBytesInReferencedBlocks>(
                    CreateRawDataAllocator(dataStream, cachedHeaderHolder),
                    new BlockBTreeEntryAllocator(
                        CreateHeaderUsageProvider(dataStream, cachedHeaderHolder),
                        allocatedBlockBTreeEntries),
                    new RegionInitializer<InternalDataBlock>(
                        dataStream,
                        null),
                    new XBlockFactory(
                        new BIDEncoder(),
                        internalBlockLevel),
                    new FuncBasedExtractor<BlockIdsWithTotalNumberOfBytesInReferencedBlocks, int>(v => v.BlockIds.Length * 8),
                    isInternal: true);
        }

        private static IDataBlockAllocator<BinaryData> CreateExternalDataBlockAllocator(
            Stream dataStream,
            List<LBBTEntry> allocatedBlockBTreeEntries,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new DataBlockAllocator<ExternalDataBlock, BinaryData>(
                    CreateRawDataAllocator(dataStream, cachedHeaderHolder),
                    new BlockBTreeEntryAllocator(
                        CreateHeaderUsageProvider(dataStream, cachedHeaderHolder),
                        allocatedBlockBTreeEntries),
                    new RegionInitializer<ExternalDataBlock>(
                        dataStream,
                        new ExternalDataBlockEncoder(
                            new BlockTrailerEncoder(
                                new BIDEncoder()))),
                    new ExternalDataBlockFactory(
                        CreateBlockEncoding(dataStream, cachedHeaderHolder)),
                    new FuncBasedExtractor<BinaryData, int>(d => d.Length),
                    isInternal: false);
        }

        private static IRawDataAllocator CreateRawDataAllocator(
            Stream dataStream,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new RawDataAllocator(
                    CreateAllocationFinder(dataStream),
                    CreateAMapBasedStreamExtender(dataStream, cachedHeaderHolder),
                    CreateAMapBasedAllocationReserver(dataStream, cachedHeaderHolder));
        }

        private static IAllocationReserver CreateAMapBasedAllocationReserver(
            Stream dataStream,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new AMapBasedAllocationReserver(
                    new RegionUpdater<AMap>(
                        dataStream,
                        CreateMapEncoder(),
                        new AMapDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder())),
                        0x200),
                    CreateHeaderUsageProvider(dataStream, cachedHeaderHolder));
        }

        private static IStreamExtender CreateAMapBasedStreamExtender(
            Stream dataStream,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new AMapBasedStreamExtender(
                    dataStream,
                    CreateMapEncoder(),
                    CreateMapEncoder(),
                    CreateMapEncoder(),
                    CreateMapEncoder(),
                    CreateHeaderUsageProvider(dataStream, cachedHeaderHolder));
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
                    new PageTrailerEncoder());
        }
    }
}
