using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using System.Collections.Generic;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeLoader
    {
        HeapOnNode Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
