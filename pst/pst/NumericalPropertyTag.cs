using System;

namespace pst
{
    public class NumericalPropertyTag
    {
        public Guid Set { get; }

        public int Id { get; }

        public PropertyType Type { get; }

        public NumericalPropertyTag(Guid set, int id, PropertyType type)
        {
            Set = set;
            Id = id;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var name = obj as NumericalPropertyTag;

            return name?.Set == Set &&
                   name.Id == Id &&
                   name.Type.Equals(Type);
        }

        public override int GetHashCode()
        {
            return $"{Set}{Id}".GetHashCode();
        }
    }
}
