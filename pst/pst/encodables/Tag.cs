using pst.utilities;
using System;

namespace pst.encodables
{
    class Tag : IComparable<Tag>
    {
        public BinaryData Value { get; }

        private Tag(BinaryData value)
        {
            Value = value;
        }

        public static Tag OfValue(BinaryData data) => new Tag(data);

        public int CompareTo(Tag other)
        {
            return Value.ToInt32().CompareTo(other.Value.ToInt32());
        }

        public override bool Equals(object obj)
        {
            var tag = obj as Tag;

            return tag?.Value.Equals(Value) ?? false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
