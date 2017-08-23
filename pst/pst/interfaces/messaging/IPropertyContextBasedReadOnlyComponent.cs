using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface IPropertyContextBasedReadOnlyComponent
    {
        Maybe<PropertyValue> GetProperty(NID[] nodePath, NumericalPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NID[] nodePath, StringPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NID[] nodePath, PropertyTag propertyTag);
    }
}