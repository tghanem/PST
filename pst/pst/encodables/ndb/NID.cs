using System;

namespace pst.encodables.ndb
{
    class NID
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
