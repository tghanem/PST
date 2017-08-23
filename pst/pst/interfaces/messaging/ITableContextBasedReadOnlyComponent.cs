using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.messaging
{
    interface ITableContextBasedReadOnlyComponent<TComponentId>
    {
        Maybe<PropertyValue> GetProperty(NID[] subnodePath, TComponentId componentId, NumericalPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NID[] subnodePath, TComponentId componentId, StringPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NID[] subnodePath, TComponentId componentId, PropertyTag propertyTag);
    }
}