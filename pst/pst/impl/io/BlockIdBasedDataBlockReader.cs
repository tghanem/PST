using pst.interfaces.io;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.io
{
    class BlockIdBasedDataBlockReader : IDataBlockReader
    {
        private readonly IDataReader dataReader;
        private readonly IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapper;

        public BlockIdBasedDataBlockReader(IDataReader dataReader, IMapper<BID, LBBTEntry> blockIdToLBBTEntryMapper)
        {
            this.dataReader = dataReader;
            this.blockIdToLBBTEntryMapper = blockIdToLBBTEntryMapper;
        }

        public BinaryData Read(BID blockId)
        {
            var lbbtEntry =
                blockIdToLBBTEntryMapper.Map(blockId);

            return
                dataReader.Read(
                    lbbtEntry.BlockReference.ByteIndex.Value,
                    lbbtEntry.GetBlockSize());
        }
    }
}
