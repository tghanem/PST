using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.impl.io
{
    class BlockIdBasedDataBlockReader : IDataBlockReader
    {
        private readonly IDataReader dataReader;
        private readonly IHeaderReader headerReader;
        private readonly IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder;

        public BlockIdBasedDataBlockReader(
            IDataReader dataReader,
            IHeaderReader headerReader,
            IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder)
        {
            this.dataReader = dataReader;
            this.headerReader = headerReader;
            this.blockBTreeEntryFinder = blockBTreeEntryFinder;
        }

        public BinaryData Read(BID blockId)
        {
            var header = headerReader.GetHeader();

            var lbbtEntry = blockBTreeEntryFinder.Find(blockId, header.Root.BBTRootPage);

            return
                dataReader.Read(
                    lbbtEntry.Value.BlockReference.ByteIndex.Value,
                    lbbtEntry.Value.ByteCountOfRawDataInReferencedBlockExcludingTrailerAndAlignmentPadding);
        }
    }
}
