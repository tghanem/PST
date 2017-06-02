using pst.core;
using pst.encodables.ndb.btree;

namespace pst.interfaces.ltp.pc
{
    interface IPCBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(LBBTEntry blockEntry, PropertyTag propertyTag);
    }
}
