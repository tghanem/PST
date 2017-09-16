using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.messaging;
using pst.interfaces.ndb;

namespace pst.impl.messaging
{
    class TableContextBasedReadOnlyComponent<TComponentId> : ITableContextBasedReadOnlyComponent<TComponentId>
    {
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly ITableContextBasedPropertyReader<TComponentId> propertyReader;

        public TableContextBasedReadOnlyComponent(
            INodeEntryFinder nodeEntryFinder,
            IPropertyNameToIdMap propertyNameToIdMap,
            ITableContextBasedPropertyReader<TComponentId> propertyReader)
        {
            this.nodeEntryFinder = nodeEntryFinder;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyReader = propertyReader;
        }

        public Maybe<PropertyValue> GetProperty(NodePath subnodePath, TComponentId componentId, NumericalPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var entry = nodeEntryFinder.GetEntry(subnodePath);

            if (entry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    entry.Value.NodeDataBlockId,
                    entry.Value.SubnodeDataBlockId,
                    componentId,
                    new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(NodePath subnodePath, TComponentId componentId, StringPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var entry = nodeEntryFinder.GetEntry(subnodePath);

            if (entry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    entry.Value.NodeDataBlockId,
                    entry.Value.SubnodeDataBlockId,
                    componentId,
                    new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(NodePath subnodePath, TComponentId componentId, PropertyTag propertyTag)
        {
            var entry = nodeEntryFinder.GetEntry(subnodePath);

            if (entry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    entry.Value.NodeDataBlockId,
                    entry.Value.SubnodeDataBlockId,
                    componentId,
                    propertyTag);
        }
    }
}
