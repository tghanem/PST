using pst.core;
using pst.interfaces.ndb;

namespace pst.interfaces.messaging
{
    interface ITableContextBasedReadOnlyComponent<TComponentId>
    {
        Maybe<PropertyValue> GetProperty(NodePath subnodePath, TComponentId componentId, NumericalPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NodePath subnodePath, TComponentId componentId, StringPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NodePath subnodePath, TComponentId componentId, PropertyTag propertyTag);
    }
}