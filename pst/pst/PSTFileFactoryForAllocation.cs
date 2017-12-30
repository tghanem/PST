using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.encodables.ndb.maps;
using pst.impl;
using pst.impl.blockallocation.datatree;
using pst.impl.decoders.ndb;
using pst.impl.encoders.ndb;
using pst.impl.encoders.ndb.blocks;
using pst.impl.encoders.ndb.btree;
using pst.impl.encoders.ndb.maps;
using pst.impl.encoders.primitives;
using pst.impl.io;
using pst.impl.rawallocation;
using pst.interfaces.blockallocation.datatree;
using pst.interfaces.rawallocation;
using pst.utilities;
using System.Collections.Generic;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IDataTreeAllocator CreateDataTreeAllocator(
            Stream dataStream,
            List<LBBTEntry> allocatedBlockBTreeEntries)
        {
            return
                new DataTreeAllocator(
                    CreateExternalDataBlockAllocator(dataStream, allocatedBlockBTreeEntries),
                    CreateInternalDataBlockAllocator(dataStream, allocatedBlockBTreeEntries, internalBlockLevel: 0x01),
                    CreateInternalDataBlockAllocator(dataStream, allocatedBlockBTreeEntries, internalBlockLevel: 0x02));
        }

        private static IDataBlockAllocator<BlockIdsWithTotalNumberOfBytesInReferencedBlocks> CreateInternalDataBlockAllocator(
            Stream dataStream,
            List<LBBTEntry> allocatedBlockBTreeEntries,
            int internalBlockLevel)
        {
            return
                new DataBlockAllocator<InternalDataBlock, BlockIdsWithTotalNumberOfBytesInReferencedBlocks>(
                    CreateRawDataAllocator(dataStream),
                    new BlockBTreeEntryAllocator(
                        CreateHeaderUsageProvider(dataStream),
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
            List<LBBTEntry> allocatedBlockBTreeEntries)
        {
            return
                new DataBlockAllocator<ExternalDataBlock, BinaryData>(
                    CreateRawDataAllocator(dataStream),
                    new BlockBTreeEntryAllocator(
                        CreateHeaderUsageProvider(dataStream),
                        allocatedBlockBTreeEntries),
                    new RegionInitializer<ExternalDataBlock>(
                        dataStream,
                        new ExternalDataBlockEncoder(
                            new BlockTrailerEncoder(
                                new BIDEncoder()))),
                    new ExternalDataBlockFactory(
                        CreateBlockEncoding(dataStream)),
                    new FuncBasedExtractor<BinaryData, int>(d => d.Length),
                    isInternal: false);
        }

        private static IRawDataAllocator CreateRawDataAllocator(
            Stream dataStream)
        {
            return
                new RawDataAllocator(
                    CreateAllocationFinder(dataStream),
                    CreateAMapBasedStreamExtender(dataStream),
                    CreateAMapBasedAllocationReserver(dataStream));
        }

        private static IAllocationReserver CreateAMapBasedAllocationReserver(
            Stream dataStream)
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
