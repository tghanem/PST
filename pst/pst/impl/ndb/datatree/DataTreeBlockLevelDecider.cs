using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ndb;

namespace pst.impl.ndb.datatree
{
    class DataTreeBlockLevelDecider : IDataTreeBlockLevelDecider
    {
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;

        public DataTreeBlockLevelDecider(
            IDataBlockReader<LBBTEntry> dataBlockReader,
            IMapper<BID, LBBTEntry> bidToLBBTEntryMapper)
        {
            this.dataBlockReader = dataBlockReader;
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
        }

        public int GetBlockLevel(BID blockId)
        {
            var lbbtEntry = bidToLBBTEntryMapper.Map(blockId);

            var dataBlock = dataBlockReader.Read(lbbtEntry, lbbtEntry.GetBlockSize());

            if (dataBlock.Value[0] == 0x01)
            {
                return dataBlock.Value[1];
            }

            return 0;
        }
    }
}
