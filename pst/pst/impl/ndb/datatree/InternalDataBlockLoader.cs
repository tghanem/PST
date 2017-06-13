using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.io;

namespace pst.impl.ndb.datatree
{
    class InternalDataBlockLoader : IBTreeNodeLoader<InternalDataBlock, BID>
    {
        private readonly IDecoder<InternalDataBlock> internalDataBlockDecoder;
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;
        private readonly IDataBlockReader<LBBTEntry> dataBlockReader;

        public InternalDataBlockLoader(
            IDecoder<InternalDataBlock> internalDataBlockDecoder,
            IMapper<BID, LBBTEntry> bidToLBBTEntryMapper,
            IDataBlockReader<LBBTEntry> dataBlockReader)
        {
            this.internalDataBlockDecoder = internalDataBlockDecoder;
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
            this.dataBlockReader = dataBlockReader;
        }

        public InternalDataBlock LoadNode(BID nodeReference)
        {
            var lbbtEntry = bidToLBBTEntryMapper.Map(nodeReference);

            var encodedBlock =
                dataBlockReader.Read(lbbtEntry, lbbtEntry.GetBlockSize());

            return internalDataBlockDecoder.Decode(encodedBlock);
        }
    }
}
