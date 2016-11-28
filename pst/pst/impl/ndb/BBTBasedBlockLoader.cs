using pst.encodables.ndb.blocks.data;
using pst.interfaces.ndb;
using pst.encodables.ndb;
using pst.interfaces.btree;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.utilities;
using pst.core;
using pst.interfaces;

namespace pst.impl.ndb
{
    /// TODO: should change the signature to return a binary data instead.
    /// TODO: if the size of the block is larger than 8K then we have a data tree.
    class BBTBasedBlockLoader : IDataBlockLoader<ExternalDataBlock>
    {
        private readonly IBTreeKeyFinder<LBBTEntry, BID> bbtEntryFinder;

        private readonly IDataReader dataReader;

        private readonly IDecoder<ExternalDataBlock> externalDataBlockDecoder;

        public BBTBasedBlockLoader(IBTreeKeyFinder<LBBTEntry, BID> bbtEntryFinder, IDataReader dataReader, IDecoder<ExternalDataBlock> externalDataBlockDecoder)
        {
            this.bbtEntryFinder = bbtEntryFinder;
            this.dataReader = dataReader;
            this.externalDataBlockDecoder = externalDataBlockDecoder;
        }

        public Maybe<ExternalDataBlock> Load(BID blockId)
        {
            var entry = bbtEntryFinder.Find(blockId);

            if (entry.HasNoValue)
            {
                return Maybe<ExternalDataBlock>.NoValue<ExternalDataBlock>();
            }

            return Maybe<ExternalDataBlock>.NoValue<ExternalDataBlock>();
        }
    }
}
