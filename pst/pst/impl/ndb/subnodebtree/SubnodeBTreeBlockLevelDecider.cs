using pst.encodables.ndb;
using pst.interfaces.io;
using pst.interfaces.ndb;

namespace pst.impl.ndb.subnodebtree
{
    class SubnodeBTreeBlockLevelDecider : ISubnodeBTreeBlockLevelDecider
    {
        private readonly IDataBlockReader dataBlockReader;

        public SubnodeBTreeBlockLevelDecider(IDataBlockReader dataBlockReader)
        {
            this.dataBlockReader = dataBlockReader;
        }

        public int GetBlockLevel(BID blockId)
        {
            var dataBlock = dataBlockReader.Read(blockId);

            return dataBlock.Value[1];
        }
    }
}
