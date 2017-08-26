using pst.core;

namespace pst.interfaces.messaging
{
    interface IPropertyContextBasedReadOnlyComponent
    {
        Maybe<PropertyValue> GetProperty(NodePath nodePath, NumericalPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NodePath nodePath, StringPropertyTag propertyTag);

        Maybe<PropertyValue> GetProperty(NodePath nodePath, PropertyTag propertyTag);
    }
}