using pst.interfaces.ndb;
using pst.utilities;

namespace pst.interfaces.ltp
{
    interface IPropertyValueReader
    {
        PropertyValue Read(NodePath nodePath, PropertyType propertyType, BinaryData propertyValue);
    }
}
