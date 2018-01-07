using pst.core;

namespace pst.interfaces.messaging.model.changetracking
{
    class PropertyTrackingObject
    {
        public PropertyTrackingObject(PropertyStates state, Maybe<PropertyValue> value)
        {
            State = state;
            Value = value;
        }

        public PropertyStates State { get; }

        public Maybe<PropertyValue> Value { get; }

        public PropertyTrackingObject SetState(PropertyStates value)
        {
            return new PropertyTrackingObject(value, Value);
        }

        public PropertyTrackingObject SetValue(PropertyValue value)
        {
            return new PropertyTrackingObject(State, value);
        }
    }
}