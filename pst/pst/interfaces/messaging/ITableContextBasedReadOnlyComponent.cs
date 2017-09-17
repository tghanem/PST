using pst.core;

namespace pst.interfaces.messaging
{
    interface ITableContextBasedReadOnlyComponent<TComponentId>
    {
        Maybe<PropertyValue> GetProperty(TComponentId componentId, NumericalTaggedPropertyPath propertyPath);

        Maybe<PropertyValue> GetProperty(TComponentId componentId, StringTaggedPropertyPath propertyPath);

        Maybe<PropertyValue> GetProperty(TComponentId componentId, TaggedPropertyPath propertyPath);
    }
}