using pst.encodables.ndb;
using pst.interfaces.io;
using pst.interfaces.ndb;

namespace pst.impl.ndb.datatree
{
    class DataTreeBlockLevelDecider : IDataTreeBlockLevelDecider
    {
        private readonly IDataBlockReader dataBlockReader;

        public DataTreeBlockLevelDecider(IDataBlockReader dataBlockReader)
        {
            this.dataBlockReader = dataBlockReader;
        }

        public int GetBlockLevel(BID blockId)
        {
            var dataBlock = dataBlockReader.Read(blockId);

            if (dataBlock.Value[0] == 0x01)
            {
                return dataBlock.Value[1];
            }

            return 0;
        }
    }
}
