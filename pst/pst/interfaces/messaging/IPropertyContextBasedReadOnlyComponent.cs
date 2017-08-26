using pst.core;

namespace pst.interfaces.messaging
{
    interface IPropertyContextBasedReadOnlyComponent
    {
        Maybe<PropertyValue> GetProperty(NumericalTaggedPropertyPath propertyPath);

        Maybe<PropertyValue> GetProperty(StringTaggedPropertyPath propertyPath);

        Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath);
    }
}