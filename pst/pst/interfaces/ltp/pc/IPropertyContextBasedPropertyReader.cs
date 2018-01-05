using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.pc
{
    interface IPropertyContextBasedPropertyReader
    {
        Maybe<PropertyValue> Read(NID[] nodePath, PropertyTag propertyTag);
    }
}
