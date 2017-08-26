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

            var nodeEntry = nodeEntryFinder.GetEntry(propertyPath.NodePath);

            if (nodeEntry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodeEntry.Value.NodeDataBlockId,
                    nodeEntry.Value.SubnodeDataBlockId,
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

            var nodeEntry = nodeEntryFinder.GetEntry(propertyPath.NodePath);

            if (nodeEntry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodeEntry.Value.NodeDataBlockId,
                    nodeEntry.Value.SubnodeDataBlockId,
                    new PropertyTag(propertyId.Value, propertyPath.PropertyTag.Type));
        }

        public Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath)
        {
            var nodeEntry = nodeEntryFinder.GetEntry(propertyPath.NodePath);

            if (nodeEntry.HasNoValue)
            {
                return Maybe<PropertyValue>.NoValue();
            }

            return
                propertyReader.ReadProperty(
                    nodeEntry.Value.NodeDataBlockId,
                    nodeEntry.Value.SubnodeDataBlockId,
                    propertyPath.PropertyTag);
        }
    }
}
