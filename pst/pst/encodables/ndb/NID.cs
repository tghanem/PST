using System;

namespace pst.encodables.ndb
{
    class NID : IComparable<NID>
    {
        public int Type { get; }

        public int Index { get; }

        public NID(int value)
        {
            Type = value & 0x0000001F;

            Index = value >> 5;
        }

        public NID(int type, int index)
        {
            Type = type;
            Index = index;
        }

        public int Value => Index << 5 | Type;

        public bool IsZero => Value == 0;

        public NID ChangeType(int type) => new NID(type, Index);

        public int CompareTo(NID other)
        {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            var nid = obj as NID;

            return nid?.Value == Value;
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
