using System;

namespace pst
{
    public class PropertyId : IComparable<PropertyId>
    {
        public int Value { get; }

        public PropertyId(int value)
        {
            Value = value;
        }

        public static PropertyId OfValue(int value) => new PropertyId(value);

        public int CompareTo(PropertyId other)
        {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            var propertyId = obj as PropertyId;

            return propertyId?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"0x{Value.ToString("x")}".ToLower();
        }
    }
}
