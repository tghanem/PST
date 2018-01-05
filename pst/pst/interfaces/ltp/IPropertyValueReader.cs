using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ltp
{
    interface IPropertyValueReader
    {
        PropertyValue Read(NID[] nodePath, PropertyType propertyType, BinaryData propertyValue);
    }
}
