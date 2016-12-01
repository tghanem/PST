using pst.core;
using pst.encodables.ndb.btree;
using pst.impl.decoders.ndb;
using pst.impl.decoders.ndb.blocks;
using pst.impl.decoders.ndb.blocks.data;
using pst.impl.decoders.primitives;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ndb;
using pst.utilities;
using System;

namespace pst.impl.ndb
{
    class OrderedDataBlockCollectionFactory : IFactory<LBBTEntry, Maybe<IOrderedDataBlockCollection>>
    {
        private readonly IDataReader dataReader;

        public OrderedDataBlockCollectionFactory(IDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        public Maybe<IOrderedDataBlockCollection> Create(LBBTEntry parameter)
        {
            var blockSize = GetBlockSize(parameter);

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
                        parameter.BlockReference.ByteIndex,
                        GetBlockSize(parameter));
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
