using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.ndb.subnodebtree
{
    class SubnodeBlockLoader : IBTreeNodeLoader<SubnodeBlock, BID>
    {
        private readonly IDecoder<SubnodeBlock> subnodeBlockDecoder;
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        public SubnodeBlockLoader(
            IDecoder<SubnodeBlock> subnodeBlockDecoder,
            IMapper<BID, LBBTEntry> bidToLBBTEntryMapper,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
            this.subnodeBlockDecoder = subnodeBlockDecoder;
            this.dataBlockReader = dataBlockReader;
        }

        public SubnodeBlock LoadNode(BID nodeReference)
        {
            var lbbtEntry =
                bidToLBBTEntryMapper.Map(nodeReference);

            var encodedBlock =
                dataBlockReader.Read(lbbtEntry, lbbtEntry.GetBlockSize());

            return subnodeBlockDecoder.Decode(encodedBlock);
        }
    }
}
