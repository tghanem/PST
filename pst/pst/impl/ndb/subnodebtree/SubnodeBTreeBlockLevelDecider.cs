using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.io;
using pst.interfaces.ndb;

namespace pst.impl.ndb.subnodebtree
{
    class SubnodeBTreeBlockLevelDecider : ISubnodeBTreeBlockLevelDecider
    {
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;

        public SubnodeBTreeBlockLevelDecider(IDataBlockReader<LBBTEntry> dataBlockReader, IMapper<BID, LBBTEntry> bidToLBBTEntryMapper)
        {
            this.dataBlockReader = dataBlockReader;
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
        }

        public int GetBlockLevel(BID blockId)
        {
            var lbbtEntry = bidToLBBTEntryMapper.Map(blockId);

            var dataBlock = dataBlockReader.Read(lbbtEntry, lbbtEntry.GetBlockSize());

            return dataBlock.Value[1];
        }
    }
}
