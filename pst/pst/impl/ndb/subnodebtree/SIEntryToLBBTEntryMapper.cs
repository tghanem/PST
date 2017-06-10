using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;

namespace pst.impl.ndb.subnodebtree
{
    class SIEntryToLBBTEntryMapper : IMapper<SIEntry, LBBTEntry>
    {
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;

        public SIEntryToLBBTEntryMapper(IMapper<BID, LBBTEntry> bidToLBBTEntryMapper)
        {
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
        }

        public LBBTEntry Map(SIEntry input)
        {
            return bidToLBBTEntryMapper.Map(input.SLBlockId);
        }
    }
}
