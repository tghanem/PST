using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.impl;
using pst.impl.btree;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.blocks;
using pst.impl.decoders.ndb.blocks.data;
using pst.impl.decoders.ndb.btree;
using pst.impl.io;
using pst.impl.ndb;
using pst.impl.ndb.bbt;
using pst.impl.ndb.datatree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;
using System.IO;

namespace pst
{
    public partial class PSTFile
    {
        private static IDataBlockReader CreateDataBlockReader(
            Stream dataReader,
            ICache<BID, BTPage> cachedBTPages,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new BlockIdBasedDataBlockReader(
                    new DataReader(dataReader),
                    CreateHeaderUsageProvider(dataReader, cachedHeaderHolder), 
                    CreateBlockBTreeEntryFinder(dataReader, cachedBTPages));
        }

        private static IDataTreeReader CreateDataTreeReader(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return
                new DataTreeReader(
                    CreateNodeEntryFinder(dataStream, cachedBTPages, cachedHeaderHolder),
                    CreateDataBlockReader(dataStream, cachedBTPages, cachedHeaderHolder),
                    CreateBlockEncoding(dataStream, cachedHeaderHolder),
                    CreateHeaderUsageProvider(dataStream, cachedHeaderHolder),
                    CreateBlockBTreeEntryFinder(dataStream, cachedBTPages),
                    CreateExternalDataBlockIdsReader(dataStream, cachedBTPages, cachedInternalDataBlocks, cachedHeaderHolder));
        }

        private static IExternalDataBlockIdsReader CreateExternalDataBlockIdsReader(
            Stream dataReader,
            ICache<BID, BTPage> cachedBTPages,
            ICache<BID, InternalDataBlock> cachedInternalDataBlocks,
            IDataHolder<Header> cachedHeaderHolder)
        {
            return 
                new ExternalDataBlockIdsReader(
                    new DataReader(dataReader),
                    CreateHeaderUsageProvider(dataReader, cachedHeaderHolder),
                    new BIDsFromInternalDataBlockExtractor(
                        new BIDDecoder()),
                    new InternalDataBlockLoader(
                        cachedInternalDataBlocks,
                        new DataReader(dataReader),
                        new InternalDataBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder()))),
                    CreateBlockBTreeEntryFinder(dataReader, cachedBTPages));
        }

        private static IBTreeEntryFinder<BID, LBBTEntry, BREF> CreateBlockBTreeEntryFinder(
            Stream dataStream,
            ICache<BID, BTPage> cachedBTPages)
        {
            return
                new BTreeEntryFinder<BID, LBBTEntry, IBBTEntry, BREF, BTPage>(
                    new FuncBasedExtractor<LBBTEntry, BID>(
                        entry => entry.BlockReference.BlockId),
                    new FuncBasedExtractor<IBBTEntry, BID>(
                        entry => entry.Key),
                    new FuncBasedExtractor<IBBTEntry, BREF>(
                        entry => entry.ChildPageBlockReference),
                    new IBBTEntriesFromBTPageExtractor(
                        new IBBTEntryDecoder(
                            new BIDDecoder(),
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new LBBTEntriesFromBTPageExtractor(
                        new LBBTEntryDecoder(
                            new BREFDecoder(
                                new BIDDecoder(),
                                new IBDecoder()))),
                    new FuncBasedExtractor<BTPage, int>(
                        page => page.PageLevel),
                    new BTPageLoader(
                        new DataReader(dataStream),
                        new BTPageDecoder(
                            new PageTrailerDecoder(
                                new BIDDecoder())),
                        cachedBTPages));
        }
    }
}
