using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces;
using System.Collections.Generic;

namespace pst.impl.ndb.subnodebtree
{
    class SIEntryToLBBTEntryMapper : IMapper<SIEntry, LBBTEntry>
    {
        private readonly IReadOnlyDictionary<BID, LBBTEntry> blockBTree;

        public SIEntryToLBBTEntryMapper(IReadOnlyDictionary<BID, LBBTEntry> blockBTree)
        {
            this.blockBTree = blockBTree;
        }

        public LBBTEntry Map(SIEntry input)
        {
            return blockBTree[input.SLBlockId];
        }
    }
}
