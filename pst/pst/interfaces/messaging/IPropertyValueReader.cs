using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.messaging
{
    interface IPropertyValueReader
    {
        PropertyValue Read(NID[] nodePath, PropertyType propertyType, BinaryData propertyValue);
    }
}
