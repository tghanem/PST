using pst.core;
using pst.interfaces.ndb;

namespace pst.interfaces.ltp.pc
{
    interface IPropertyContextBasedPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(NodePath nodePath, PropertyTag propertyTag);
    }
}
