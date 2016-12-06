using pst.encodables.ndb;
using pst.encodables.ndb.blocks.data;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using System.Collections.Generic;

namespace pst.interfaces.ndb
{
    interface IDataTreeLeafNodesEnumerator
    {
        ExternalDataBlock[] Enumerate(
            IDataBlockReader<LBBTEntry> reader,
            IReadOnlyDictionary<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
