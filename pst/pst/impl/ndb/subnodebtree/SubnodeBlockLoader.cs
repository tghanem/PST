using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;

namespace pst.impl.ndb.subnodebtree
{
    class SubnodeBlockLoader : IBTreeNodeLoader<SubnodeBlock, BID>
    {
        private readonly IDecoder<SubnodeBlock> subnodeBlockDecoder;
        private readonly IDataBlockReader dataBlockReader;

        public SubnodeBlockLoader(IDecoder<SubnodeBlock> subnodeBlockDecoder, IDataBlockReader dataBlockReader)
        {
            this.subnodeBlockDecoder = subnodeBlockDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public SubnodeBlock LoadNode(BID nodeReference)
        {
            var encodedBlock = dataBlockReader.Read(nodeReference);

            return subnodeBlockDecoder.Decode(encodedBlock);
        }
    }
}
