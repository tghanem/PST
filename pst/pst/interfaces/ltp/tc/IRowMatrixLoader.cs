using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.utilities;
using System.Collections.Generic;

namespace pst.interfaces.ltp.tc
{
    class TableRow
    {
        public IReadOnlyDictionary<int, BinaryData> Values { get; }

        public TableRow(IReadOnlyDictionary<int, BinaryData> values)
        {
            Values = values;
        }
    }

    interface IRowMatrixLoader
    {
        TableRow[] Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<NID, SLEntry> nidToSLEntryMapping,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
