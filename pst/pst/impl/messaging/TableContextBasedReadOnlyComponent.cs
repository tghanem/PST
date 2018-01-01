using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.ltp.tc;
using pst.interfaces.messaging;

namespace pst.impl.messaging
{
    class TableContextBasedReadOnlyComponent<TComponentId> : ITableContextBasedReadOnlyComponent<TComponentId>
    {
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextBasedPropertyReader<TComponentId> propertyReader;

        public TableContextBasedReadOnlyComponent(
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader<TComponentId> propertyReader)
        {
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyReader = propertyReader;
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, NumericalTaggedPropertyPath propertyPath)
        {
            var propertyId =
                propertyNameToIdMap.GetPropertyId(propertyPath.PropertyTag.Set, propertyPath.PropertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    propertyPath.NodePath,
                    componentId,
                    new PropertyTag(propertyId.Value, propertyPath.PropertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, StringTaggedPropertyPath propertyPath)
        {
            var propertyId =
                propertyNameToIdMap.GetPropertyId(propertyPath.PropertyTag.Set, propertyPath.PropertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    propertyPath.NodePath,
                    componentId,
                    new PropertyTag(propertyId.Value, propertyPath.PropertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, TaggedPropertyPath propertyPath)
        {
            return
                propertyReader.ReadProperty(
                    propertyPath.NodePath,
                    componentId,
                    propertyPath.PropertyTag);
        }
    }
}
