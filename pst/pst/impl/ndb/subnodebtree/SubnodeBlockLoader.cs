using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.ndb.subnodebtree
{
    class SubnodeBlockLoader : IBTreeNodeLoader<SubnodeBlock, LBBTEntry>
    {
        private readonly IDecoder<SubnodeBlock> subnodeBlockDecoder;

        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        public SubnodeBlockLoader(
            IDecoder<SubnodeBlock> subnodeBlockDecoder,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.subnodeBlockDecoder = subnodeBlockDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public SubnodeBlock LoadNode(LBBTEntry nodeReference)
        {
            var encodedBlock =
                dataBlockReader.Read(nodeReference, nodeReference.GetBlockSize());

            return subnodeBlockDecoder.Decode(encodedBlock);
        }
    }
}
