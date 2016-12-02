using pst.interfaces.ndb;
using pst.encodables.ndb;
using pst.interfaces.btree;
using pst.encodables.ndb.btree;
using pst.core;
using pst.utilities;
using pst.impl.decoders.ndb.blocks.data;
using pst.impl.decoders.ndb.blocks;
using pst.impl.decoders.ndb;
using pst.impl.decoders.primitives;
using pst.interfaces.io;
using System;

namespace pst.impl.ndb
{
    class OrderedNodeDataBlockCollectionLoader : IOrderedDataBlockCollectionLoader
    {
        private readonly IDataReader dataReader;
        private readonly IBTreeKeyFinder<LBBTEntry, BID> bbtEntryFinder;

        public OrderedNodeDataBlockCollectionLoader(IDataReader dataReader, IBTreeKeyFinder<LBBTEntry, BID> bbtEntryFinder)
        {
            this.dataReader = dataReader;
            this.bbtEntryFinder = bbtEntryFinder;
        }

        public Maybe<IOrderedDataBlockCollection> Load(BID blockId)
        {
            var entry = bbtEntryFinder.Find(blockId);

            if (entry.HasNoValue)
            {
                return Maybe<IOrderedDataBlockCollection>.NoValue<IOrderedDataBlockCollection>();
            }

            var blockSize = GetBlockSize(entry.Value);

            if (blockSize <= 8 * 1024)
            {
                return
                    new SingleExternalDataBlockBasedOrderedDataBlockCollection(
                        dataReader,
                        new ExternalDataBlockDecoder(
                            new BlockTrailerDecoder(
                                new BIDDecoder(),
                                new Int32Decoder()),
                            new PermutativeDecoder(false)),
                        entry.Value.BlockReference.ByteIndex,
                        blockSize);
            }

            throw new Exception("Data trees are currently not supported");
        }

        private int GetBlockSize(LBBTEntry entry)
        {
            var rawDataSize =
                entry.ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding;

            var paddingSize =
                (rawDataSize + 16).GetRemainingToNextMultipleOf(64);

            return rawDataSize + paddingSize + 16;
        }
    }
}
