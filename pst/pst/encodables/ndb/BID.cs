using System;

namespace pst.encodables.ndb
{
    class BID : IComparable<BID>, IEquatable<BID>
    {
        public long Value { get; }

        private BID(long value)
        {
            Value = value;
        }

        public static BID OfValue(long value)
            => new BID(value);

        public bool Equals(BID other)
        {
            return other.Value == Value;
        }

        public int CompareTo(BID other)
        {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            var bid = obj as BID;

            return bid.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
