using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;

namespace pst.interfaces.ltp.pc
{
    interface IPCBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            PropertyTag propertyTag);
    }
}
