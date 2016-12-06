using pst.encodables.ndb.blocks.data;
using pst.interfaces;

namespace pst.impl.ndb.datatree
{
    class NodeLevelFromInternalDataBlockExtractor
        : IExtractor<InternalDataBlock, int>
    {
        public int Extract(InternalDataBlock parameter)
        {
            return parameter.BlockLevel;
        }
    }
}
