using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IPropertyContextBasedPropertyReader
    {
        Maybe<PropertyValue> Read(NID[] nodePath, PropertyTag propertyTag);
    }
}
