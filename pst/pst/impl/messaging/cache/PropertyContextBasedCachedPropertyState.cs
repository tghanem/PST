using pst.core;

namespace pst.impl.messaging.cache
{
    class PropertyContextBasedCachedPropertyState
    {
        public PropertyOperations LastOperationOnProperty { get; }

        public Maybe<PropertyValue> LastKnownPropertyValue { get; }

        public PropertyContextBasedCachedPropertyState(PropertyOperations operationOnProperty)
        {
            LastOperationOnProperty = operationOnProperty;
        }

        public PropertyContextBasedCachedPropertyState(PropertyOperations operationOnProperty, PropertyValue propertyValue)
        {
            LastOperationOnProperty = operationOnProperty;
            LastKnownPropertyValue = propertyValue;
        }
    }
}
