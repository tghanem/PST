using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface ITableContextBasedPropertyReader
    {
        Maybe<PropertyValue> Read(NID[] nodePath, int rowId, PropertyTag propertyTag);
    }
}