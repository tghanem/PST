using pst.core;
using pst.interfaces;
using pst.interfaces.messaging;

namespace pst.impl.messaging.cache
{
    class PropertyContextBasedReadOnlyComponentThatCachesThePropertyValue : IPropertyContextBasedReadOnlyComponent
    {
        private readonly ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache;
        private readonly ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache;
        private readonly ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache;

        private readonly IPropertyContextBasedReadOnlyComponent actualPropertyContextBasedReadOnlyComponent;

        public PropertyContextBasedReadOnlyComponentThatCachesThePropertyValue(
            ICache<NumericalTaggedPropertyPath, PropertyValue> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyValue> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyValue> taggedPropertyCache,
            IPropertyContextBasedReadOnlyComponent actualPropertyContextBasedReadOnlyComponent)
        {
            this.numericalTaggedPropertyCache = numericalTaggedPropertyCache;
            this.stringTaggedPropertyCache = stringTaggedPropertyCache;
            this.taggedPropertyCache = taggedPropertyCache;
            this.actualPropertyContextBasedReadOnlyComponent = actualPropertyContextBasedReadOnlyComponent;
        }

        public Maybe<PropertyValue> GetProperty(NumericalTaggedPropertyPath propertyPath)
        {
            return
                numericalTaggedPropertyCache.GetOrAdd(
                    propertyPath,
                    () => actualPropertyContextBasedReadOnlyComponent.GetProperty(propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(StringTaggedPropertyPath propertyPath)
        {
            return
                stringTaggedPropertyCache.GetOrAdd(
                    propertyPath,
                    () => actualPropertyContextBasedReadOnlyComponent.GetProperty(propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath)
        {
            return
                taggedPropertyCache.GetOrAdd(
                    propertyPath,
                    () => actualPropertyContextBasedReadOnlyComponent.GetProperty(propertyPath));
        }
    }
}
