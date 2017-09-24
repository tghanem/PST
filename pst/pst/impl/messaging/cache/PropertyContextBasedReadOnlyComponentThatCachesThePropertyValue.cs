using pst.core;
using pst.interfaces;
using pst.interfaces.messaging;
using System;

namespace pst.impl.messaging.cache
{
    class PropertyContextBasedReadOnlyComponentThatCachesThePropertyValue : IPropertyContextBasedReadOnlyComponent
    {
        private readonly ICache<NumericalTaggedPropertyPath, PropertyContextBasedCachedPropertyState> numericalTaggedPropertyCache;
        private readonly ICache<StringTaggedPropertyPath, PropertyContextBasedCachedPropertyState> stringTaggedPropertyCache;
        private readonly ICache<TaggedPropertyPath, PropertyContextBasedCachedPropertyState> taggedPropertyCache;

        private readonly IPropertyContextBasedReadOnlyComponent actualPropertyContextBasedReadOnlyComponent;

        public PropertyContextBasedReadOnlyComponentThatCachesThePropertyValue(
            ICache<NumericalTaggedPropertyPath, PropertyContextBasedCachedPropertyState> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyContextBasedCachedPropertyState> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyContextBasedCachedPropertyState> taggedPropertyCache,
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
                GetProperty(
                    propertyPath,
                    numericalTaggedPropertyCache,
                    () => actualPropertyContextBasedReadOnlyComponent.GetProperty(propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(StringTaggedPropertyPath propertyPath)
        {
            return
                GetProperty(
                    propertyPath,
                    stringTaggedPropertyCache,
                    () => actualPropertyContextBasedReadOnlyComponent.GetProperty(propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(TaggedPropertyPath propertyPath)
        {
            return
                GetProperty(
                    propertyPath,
                    taggedPropertyCache,
                    () => actualPropertyContextBasedReadOnlyComponent.GetProperty(propertyPath));
        }

        private Maybe<PropertyValue> GetProperty<TPropertyPath>(
            TPropertyPath propertyPath,
            ICache<TPropertyPath, PropertyContextBasedCachedPropertyState> cache,
            Func<Maybe<PropertyValue>> getPropertyValue)
        {
            if (cache.HasValue(propertyPath))
            {
                var value = cache.GetValue(propertyPath);

                if (value.LastOperationOnProperty == PropertyOperations.New ||
                    value.LastOperationOnProperty == PropertyOperations.Read ||
                    value.LastOperationOnProperty == PropertyOperations.Updated)
                {
                    return value.LastKnownPropertyValue;
                }

                return Maybe<PropertyValue>.NoValue();
            }

            var propertyValue = getPropertyValue();

            if (propertyValue.HasValue)
            {
                cache.Add(
                    propertyPath,
                    new PropertyContextBasedCachedPropertyState(PropertyOperations.Read, propertyValue.Value));
            }

            return propertyValue;
        }
    }
}
