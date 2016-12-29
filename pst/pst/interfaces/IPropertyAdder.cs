using pst.utilities;

namespace pst.interfaces
{
    interface IPropertyAdder
    {
        void Add(PropertyId propertyId, PropertyType propertyType, BinaryData propertyValue);
    }
}
