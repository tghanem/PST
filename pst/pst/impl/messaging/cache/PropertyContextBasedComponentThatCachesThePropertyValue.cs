using pst.core;
using pst.interfaces;
using pst.interfaces.messaging;
using System;

namespace pst.impl.messaging.cache
{
    class PropertyContextBasedComponentThatCachesThePropertyValue : IPropertyContextBasedComponent
    {
        private readonly ICache<NumericalTaggedPropertyPath, PropertyContextBasedCachedPropertyState> numericalTaggedPropertyCache;
        private readonly ICache<StringTaggedPropertyPath, PropertyContextBasedCachedPropertyState> stringTaggedPropertyCache;
        private readonly ICache<TaggedPropertyPath, PropertyContextBasedCachedPropertyState> taggedPropertyCache;

        private readonly IPropertyContextBasedComponent actualPropertyContextBasedReadOnlyComponent;

        public PropertyContextBasedComponentThatCachesThePropertyValue(
            ICache<NumericalTaggedPropertyPath, PropertyContextBasedCachedPropertyState> numericalTaggedPropertyCache,
            ICache<StringTaggedPropertyPath, PropertyContextBasedCachedPropertyState> stringTaggedPropertyCache,
            ICache<TaggedPropertyPath, PropertyContextBasedCachedPropertyState> taggedPropertyCache,
            IPropertyContextBasedComponent actualPropertyContextBasedReadOnlyComponent)
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

        public void SetProperty(NumericalTaggedPropertyPath propertyPath, PropertyValue propertyvalue)
        {
            SetProperty(propertyPath, numericalTaggedPropertyCache, propertyvalue);
        }

        public void SetProperty(StringTaggedPropertyPath propertyPath, PropertyValue propertyvalue)
        {
            SetProperty(propertyPath, stringTaggedPropertyCache, propertyvalue);
        }

        public void SetProperty(TaggedPropertyPath propertyPath, PropertyValue propertyvalue)
        {
            SetProperty(propertyPath, taggedPropertyCache, propertyvalue);
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

        private void SetProperty<TPropertyPath>(
            TPropertyPath propertyPath,
            ICache<TPropertyPath, PropertyContextBasedCachedPropertyState> cache,
            PropertyValue newPropertyValue)
        {
            if (!cache.HasValue(propertyPath))
            {
                cache.Add(
                    propertyPath,
                    new PropertyContextBasedCachedPropertyState(PropertyOperations.New, newPropertyValue));

                return;
            }

            var oldPropertyValue = cache.GetValue(propertyPath);

            if (oldPropertyValue.LastOperationOnProperty == PropertyOperations.Deleted ||
                oldPropertyValue.LastOperationOnProperty == PropertyOperations.Read ||
                oldPropertyValue.LastOperationOnProperty == PropertyOperations.Updated)
            {
                cache.Add(
                    propertyPath,
                    new PropertyContextBasedCachedPropertyState(PropertyOperations.Updated, newPropertyValue));
            }
            else
            {
                cache.Add(
                    propertyPath,
                    new PropertyContextBasedCachedPropertyState(PropertyOperations.New, newPropertyValue));
            }
        }
    }
}
