using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.ltp.pc;
using pst.interfaces.messaging;

namespace pst.impl.messaging
{
    class PropertyContextBasedComponent : IPropertyContextBasedComponent
    {
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyContextBasedPropertyReader propertyContextBasedPropertyReader;

        public PropertyContextBasedComponent(
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
                propertyContextBasedPropertyReader.Read(
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
                propertyContextBasedPropertyReader.Read(
                    propertyPath.NodePath,
                    new PropertyTag(propertyId.Value, propertyPath.PropertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath)
        {
            return
                propertyContextBasedPropertyReader.Read(
                    propertyPath.NodePath,
                    propertyPath.PropertyTag);
        }

        public void SetProperty(NumericalTaggedPropertyPath propertyPath, PropertyValue propertyvalue)
        {
            throw new System.NotImplementedException();
        }

        public void SetProperty(StringTaggedPropertyPath propertyPath, PropertyValue propertyvalue)
        {
            throw new System.NotImplementedException();
        }

        public void SetProperty(TaggedPropertyPath propertyPath, PropertyValue propertyvalue)
        {
            throw new System.NotImplementedException();
        }
    }
}
