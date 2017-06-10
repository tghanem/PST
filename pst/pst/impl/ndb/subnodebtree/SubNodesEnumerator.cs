using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.interfaces.btree;
using pst.interfaces.ndb;

namespace pst.impl.ndb.subnodebtree
{
    class SubNodesEnumerator : ISubNodesEnumerator
    {
        private readonly IMapper<BID, LBBTEntry> bidToLBBTEntryMapper;
        private readonly IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<SLEntry, SIEntry, LBBTEntry> subnodeBTreeLeafKeysEnumerator;

        public SubNodesEnumerator(
            IMapper<BID, LBBTEntry> bidToLBBTEntryMapper,
            IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<SLEntry, SIEntry, LBBTEntry> subnodeBTreeLeafKeysEnumerator)
        {
            this.bidToLBBTEntryMapper = bidToLBBTEntryMapper;
            this.subnodeBTreeLeafKeysEnumerator = subnodeBTreeLeafKeysEnumerator;
        }
        public SLEntry[] Enumerate(BID subnodeDataBlockId)
        {
            var lbbtEntry = bidToLBBTEntryMapper.Map(subnodeDataBlockId);

            return subnodeBTreeLeafKeysEnumerator.Enumerate(lbbtEntry);
        }
    }
}
