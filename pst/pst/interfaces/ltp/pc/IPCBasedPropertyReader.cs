using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.pc
{
    interface IPCBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(NID nodeId, PropertyTag propertyTag);
    }
}
