using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.pc
{
    interface IPCBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(BID nodeDataBlockId, BID subnodeDataBlockId, PropertyTag propertyTag);
    }
}
