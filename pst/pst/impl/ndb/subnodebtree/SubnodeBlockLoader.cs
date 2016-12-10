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

        public SubnodeBlockLoader(IDecoder<SubnodeBlock> subnodeBlockDecoder)
        {
            this.subnodeBlockDecoder = subnodeBlockDecoder;
        }

        public SubnodeBlock LoadNode(IDataBlockReader<LBBTEntry> reader, LBBTEntry nodeReference)
        {
            var encodedBlock =
                reader.Read(nodeReference, nodeReference.GetBlockSize());

            return subnodeBlockDecoder.Decode(encodedBlock);
        }
    }
}
