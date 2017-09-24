using pst.core;

namespace pst.impl.messaging.cache
{
    class TableContextBasedCachedPropertyState<TComponentId>
    {
        public TComponentId ComponentId { get; }

        public PropertyOperations LastOperationOnProperty { get; }

        public Maybe<PropertyValue> LastKnownPropertyValue { get; }

        public TableContextBasedCachedPropertyState(TComponentId componentId, PropertyOperations operationOnProperty)
        {
            ComponentId = componentId;
            LastOperationOnProperty = operationOnProperty;
        }

        public TableContextBasedCachedPropertyState(TComponentId componentId, PropertyOperations operationOnProperty, PropertyValue propertyValue)
        {
            ComponentId = componentId;
            LastOperationOnProperty = operationOnProperty;
            LastKnownPropertyValue = propertyValue;
        }
    }
}
