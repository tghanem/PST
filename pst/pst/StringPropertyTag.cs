using System;

namespace pst
{
    public class StringPropertyTag
    {
        public Guid Set { get; }

        public string Name { get; }

        public PropertyType Type { get; }

        public StringPropertyTag(Guid set, string name, PropertyType type)
        {
            Set = set;
            Name = name;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var name = obj as StringPropertyTag;

            return name?.Set == Set &&
                   name.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) &&
                   name.Type.Equals(Type);
        }

        public override int GetHashCode()
        {
            var p = 17;
            p = p + 23 * Set.GetHashCode();
            p = p + 23 * Name.GetHashCode();
            return p;
        }
    }
}
