using pst.core;

namespace pst.interfaces.messaging
{
    interface IPropertyContextBasedComponent
    {
        Maybe<PropertyValue> GetProperty(NumericalTaggedPropertyPath propertyPath);

        Maybe<PropertyValue> GetProperty(StringTaggedPropertyPath propertyPath);

        Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath);

        void SetProperty(NumericalTaggedPropertyPath propertyPath, PropertyValue propertyvalue);

        void SetProperty(StringTaggedPropertyPath propertyPath, PropertyValue propertyvalue);

        void SetProperty(TaggedPropertyPath propertyPath, PropertyValue propertyvalue);
    }
}