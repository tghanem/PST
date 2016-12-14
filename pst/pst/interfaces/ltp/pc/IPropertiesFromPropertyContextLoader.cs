using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using System.Collections.Generic;

namespace pst.interfaces.ltp.pc
{
    interface IPropertiesFromPropertyContextLoader
    {
        IDictionary<PropertyId, PropertyValue> Load(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);
    }
}
