using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

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

        public Maybe<PropertyValue> GetProperty(NodePath nodePath, TComponentId componentId, NumericalPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodePath,
                    componentId,
                    new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(NodePath nodePath, TComponentId componentId, StringPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodePath,
                    componentId,
                    new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(NodePath nodePath, TComponentId componentId, PropertyTag propertyTag)
        {
            return
                propertyReader.ReadProperty(
                    nodePath,
                    componentId,
                    propertyTag);
        }
    }
}
