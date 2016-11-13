namespace pst.encodables
{
    class NID
    {
        public int Type { get; }

        public int Index { get; }

        public NID(int type, int index)
        {
            Type = type;
            Index = index;
        }
    }
}
