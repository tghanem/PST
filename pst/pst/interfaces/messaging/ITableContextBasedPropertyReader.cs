using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface ITableContextBasedPropertyReader
    {
        Maybe<PropertyValue> Read(NID[] nodePath, TCROWID rowId, PropertyTag propertyTag);
    }
}