using pst.core;
using pst.interfaces;
using pst.interfaces.messaging;
using System;

namespace pst.impl.messaging.cache
{
    class TableContextBasedReadOnlyComponentThatCachesThePropertyValue<TComponentId> : ITableContextBasedReadOnlyComponent<TComponentId>
    {
        private readonly ICache<NumericalTaggedPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> numericalTaggedPropertyCache;
        private readonly ICache<StringTaggedPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> stringTaggedPropertyCache;
        private readonly ICache<TaggedPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> taggedPropertyCache;

        private readonly ITableContextBasedReadOnlyComponent<TComponentId> actualTableContextBasedReadOnlyComponent;

        public TableContextBasedReadOnlyComponentThatCachesThePropertyValue(
            ICache<NumericalTaggedPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> taggedPropertyCache,
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
                GetProperty(
                    propertyPath,
                    componentId,
                    numericalTaggedPropertyCache,
                    () => actualTableContextBasedReadOnlyComponent.GetProperty(componentId, propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, StringTaggedPropertyPath propertyPath)
        {
            return
                GetProperty(
                    propertyPath,
                    componentId,
                    stringTaggedPropertyCache, 
                    () => actualTableContextBasedReadOnlyComponent.GetProperty(componentId, propertyPath));
        }

        public Maybe<PropertyValue> GetProperty(TComponentId componentId, TaggedPropertyPath propertyPath)
        {
            return
                GetProperty(
                    propertyPath,
                    componentId,
                    taggedPropertyCache, 
                    () => actualTableContextBasedReadOnlyComponent.GetProperty(componentId, propertyPath));
        }

        private Maybe<PropertyValue> GetProperty<TPropertyPath>(
            TPropertyPath propertyPath,
            TComponentId componentId,
            ICache<TPropertyPath, TableContextBasedCachedPropertyState<TComponentId>> cache,
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
                    new TableContextBasedCachedPropertyState<TComponentId>(componentId, PropertyOperations.Read, propertyValue.Value));
            }

            return propertyValue;
        }
    }
}
