using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;
using pst.utilities;

namespace pst.impl.io
{
    class BlockIdBasedDataBlockReader : IDataBlockReader
    {
        private readonly IDataReader dataReader;
        private readonly IDecoder<Header> headerDecoder;
        private readonly IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder;

        public BlockIdBasedDataBlockReader(
            IDataReader dataReader,
            IDecoder<Header> headerDecoder,
            IBTreeEntryFinder<BID, LBBTEntry, BREF> blockBTreeEntryFinder)
        {
            this.dataReader = dataReader;
            this.blockBTreeEntryFinder = blockBTreeEntryFinder;
            this.headerDecoder = headerDecoder;
        }

        public BinaryData Read(BID blockId)
        {
            var header = headerDecoder.Decode(dataReader.Read(0, 546));

            var lbbtEntry = blockBTreeEntryFinder.Find(blockId, header.Root.BBTRootPage);

            return dataReader.Read(lbbtEntry.Value.BlockReference.ByteIndex.Value, lbbtEntry.Value.GetBlockSize());
        }
    }
}
