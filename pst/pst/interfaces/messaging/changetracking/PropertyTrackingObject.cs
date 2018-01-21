using pst.core;

namespace pst.interfaces.messaging.changetracking
{
    class PropertyTrackingObject
    {
        public PropertyTrackingObject(PropertyTag tag, PropertyStates state, Maybe<PropertyValue> value)
        {
            Tag = tag;
            State = state;
            Value = value;
        }

        public PropertyTag Tag { get; }

        public PropertyStates State { get; }

        public Maybe<PropertyValue> Value { get; }

        public PropertyTrackingObject SetState(PropertyStates value)
        {
            return new PropertyTrackingObject(Tag, value, Value);
        }

        public PropertyTrackingObject SetValue(PropertyValue value)
        {
            return new PropertyTrackingObject(Tag, State, value);
        }

        public override bool Equals(object obj)
        {
            var trackingObject = obj as PropertyTrackingObject;

            return trackingObject?.Tag.Equals(Tag) ?? false;
        }

        public override int GetHashCode() => Tag.GetHashCode();
    }
}