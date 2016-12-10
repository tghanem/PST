using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;

namespace pst.impl.ndb.subnodebtree
{
    class NodeLevelFromSubnodeBlockExtractor : IExtractor<SubnodeBlock, int>
    {
        public int Extract(SubnodeBlock parameter)
        {
            return parameter.BlockLevel;
        }
    }
}
