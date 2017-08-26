using pst.core;
using pst.interfaces.ltp;
using pst.interfaces.messaging;

namespace pst.impl.messaging
{
    class PropertyContextBasedReadOnlyComponent : IPropertyContextBasedReadOnlyComponent
    {
        private readonly INodeEntryFinder nodeEntryFinder;
        private readonly IPropertyNameToIdMap propertyNameToIdMap;
        private readonly IPropertyReader propertyReader;

        public PropertyContextBasedReadOnlyComponent(
            INodeEntryFinder nodeEntryFinder,
            IPropertyNameToIdMap propertyNameToIdMap,
            IPropertyReader propertyReader)
        {
            this.nodeEntryFinder = nodeEntryFinder;
            this.propertyNameToIdMap = propertyNameToIdMap;
            this.propertyReader = propertyReader;
        }

        public Maybe<PropertyValue> GetProperty(NodePath nodePath, NumericalPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Id);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

            if (nodeEntry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodeEntry.Value.NodeDataBlockId,
                    nodeEntry.Value.SubnodeDataBlockId,
                    new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(NodePath nodePath, StringPropertyTag propertyTag)
        {
            var propertyId = propertyNameToIdMap.GetPropertyId(propertyTag.Set, propertyTag.Name);

            if (propertyId.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

            if (nodeEntry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodeEntry.Value.NodeDataBlockId,
                    nodeEntry.Value.SubnodeDataBlockId,
                    new PropertyTag(propertyId.Value, propertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(NodePath nodePath, PropertyTag propertyTag)
        {
            var nodeEntry = nodeEntryFinder.GetEntry(nodePath);

            if (nodeEntry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodeEntry.Value.NodeDataBlockId,
                    nodeEntry.Value.SubnodeDataBlockId,
                    propertyTag);
        }
    }
}
