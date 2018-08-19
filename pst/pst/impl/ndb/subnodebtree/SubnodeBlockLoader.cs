using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;

namespace pst.impl.ndb.subnodebtree
{
    class SubnodeBlockLoader : IBTreeNodeLoader<SubnodeBlock, BID>
    {
        private readonly ICache<BID, SubnodeBlock> cachedSubnodeBlocks;
        private readonly IDataBlockReader dataBlockReader;
        private readonly IDecoder<SubnodeBlock> subnodeBlockDecoder;

        public SubnodeBlockLoader(ICache<BID, SubnodeBlock> cachedSubnodeBlocks, IDataBlockReader dataBlockReader, IDecoder<SubnodeBlock> subnodeBlockDecoder)
        {
            this.subnodeBlockDecoder = subnodeBlockDecoder;
            this.dataBlockReader = dataBlockReader;
            this.cachedSubnodeBlocks = cachedSubnodeBlocks;
        }

        public SubnodeBlock LoadNode(BID nodeReference)
        {
            return
                cachedSubnodeBlocks
                .GetOrAdd(
                    nodeReference,
                    () =>
                    {
                        var encodedBlock = dataBlockReader.Read(nodeReference);

                        return subnodeBlockDecoder.Decode(encodedBlock);
                    })
                .Value;
        }
    }
}
