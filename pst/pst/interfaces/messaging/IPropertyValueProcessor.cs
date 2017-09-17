using pst.interfaces.ndb;
using pst.utilities;

namespace pst.interfaces.messaging
{
    interface IPropertyValueProcessor
    {
        PropertyValue Process(NodePath nodePath, PropertyType propertyType, BinaryData propertyValue);
    }
}
