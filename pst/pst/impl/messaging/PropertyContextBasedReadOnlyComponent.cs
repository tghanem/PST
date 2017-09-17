using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.messaging;

namespace pst.impl.messaging
{
    class PropertyContextBasedReadOnlyComponent : IPropertyContextBasedReadOnlyComponent
    {
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;

        public PropertyContextBasedReadOnlyComponent(
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader)
        {
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyContextBasedPropertyReader = propertyContextBasedPropertyReader;
        }

        public Maybe<PropertyValue> GetProperty(NumericalTaggedPropertyPath propertyPath)
        {
            var propertyId =
                propertyNameToIdMap.GetPropertyId(
                    propertyPath.PropertyTag.Set,
                    propertyPath.PropertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyContextBasedPropertyReader.ReadProperty(
                    propertyPath.NodePath,
                    new PropertyTag(propertyId.Value, propertyPath.PropertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(StringTaggedPropertyPath propertyPath)
        {
            var propertyId =
                propertyNameToIdMap.GetPropertyId(
                    propertyPath.PropertyTag.Set,
                    propertyPath.PropertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyContextBasedPropertyReader.ReadProperty(
                    propertyPath.NodePath,
                    new PropertyTag(propertyId.Value, propertyPath.PropertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath)
        {
            return
                propertyContextBasedPropertyReader.ReadProperty(
                    propertyPath.NodePath,
                    propertyPath.PropertyTag);
        }
    }
}
