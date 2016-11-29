using System;

namespace pst
{
    public class PropertyId : IComparable<PropertyId>, IEquatable<PropertyId>
    {
        public int Value { get; }

        public PropertyId(int value)
        {
            Value = value;
        }

        public int CompareTo(PropertyId other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(PropertyId other)
        {
            return Value == other.Value;
        }
    }
}
