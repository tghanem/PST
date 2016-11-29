using System;

namespace pst.encodables.ndb
{
    class NID : IComparable<NID>, IEquatable<NID>
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

        public int Value => Index | Type << 5;

        public int CompareTo(NID other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(NID other)
        {
            return other.Value == Value;
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
    }
}
