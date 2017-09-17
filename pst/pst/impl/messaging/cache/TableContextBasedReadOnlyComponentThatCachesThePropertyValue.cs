using pst.core;
using pst.interfaces;
using pst.interfaces.messaging;

namespace pst.impl.messaging.cache
{
    class TableContextBasedReadOnlyComponentThatCachesThePropertyValue<TComponentId> : ITableContextBasedReadOnlyComponent<TComponentId>
    {
        private readonly ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache;
        private readonly ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache;
        private readonly ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache;

        private readonly ITableContextBasedReadOnlyComponent<TComponentId> actualTableContextBasedReadOnlyComponent;

        public TableContextBasedReadOnlyComponentThatCachesThePropertyValue(
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache,
            ITableContextBasedReadOnlyComponent<TComponentId> actualTableContextBasedReadOnlyComponent)
        {
            this.numericalTaggedPropertyCache = numericalTaggedPropertyCache;
            this.stringTaggedPropertyCache = stringTaggedPropertyCache;
            this.taggedPropertyCache = taggedPropertyCache;
            this.actualTableContextBasedReadOnlyComponent = actualTableContextBasedReadOnlyComponent;
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, NumericalTaggedPropertyPath propertyPath)
        {
            return
                numericalTaggedPropertyCache.GetOrAdd(
                    propertyPath,
                    () => actualTableContextBasedReadOnlyComponent.GetProperty(componentId, propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, StringTaggedPropertyPath propertyPath)
        {
            return
                stringTaggedPropertyCache.GetOrAdd(
                    propertyPath,
                    () => actualTableContextBasedReadOnlyComponent.GetProperty(componentId, propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, TaggedPropertyPath propertyPath)
        {
            return
                taggedPropertyCache.GetOrAdd(
                    propertyPath,
                    () => actualTableContextBasedReadOnlyComponent.GetProperty(componentId, propertyPath));
        }
    }
}
