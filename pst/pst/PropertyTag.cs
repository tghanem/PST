using System;

namespace pst
{
    public class PropertyTag : IComparable<PropertyTag>
    {
        public PropertyId Id { get; }

        public PropertyType Type { get; }

        public int Value => (Id.Value << 16) | Type.Value;

        public PropertyTag(PropertyId id, PropertyType type)
        {
            Id = id;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var tag = obj as PropertyTag;

            return tag?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(PropertyTag other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return $"0x{Value.ToString("x")}".ToLower();
        }
    }
}
