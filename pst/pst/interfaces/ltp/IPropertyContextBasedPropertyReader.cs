using pst.core;
using pst.interfaces.ndb;

namespace pst.interfaces.ltp
{
    interface IPropertyContextBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(NodePath nodePath, PropertyTag propertyTag);
    }
}
