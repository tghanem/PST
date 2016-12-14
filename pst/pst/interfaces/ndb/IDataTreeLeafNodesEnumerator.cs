using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces.io;

namespace pst.interfaces.ndb
{
    interface IDataTreeLeafNodesEnumerator
    {
        ExternalDataBlock[] Enumerate(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
