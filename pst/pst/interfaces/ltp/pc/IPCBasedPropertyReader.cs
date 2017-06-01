using pst.core;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.interfaces.ltp.pc
{
    interface IPCBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            PropertyTag propertyTag);
    }
}
