using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.messaging
{
    interface IPropertyValueProcessor
    {
        PropertyValue Process(BID dataBlockId, BID subnodeDataBlockId, PropertyType propertyType, BinaryData propertyValue);
    }
}
