namespace pst.encodables
{
    class INBTEntry
    {
        public NID Key { get; }

        public BREF ChildPageBlockReference { get; }

        private INBTEntry(NID key, BREF childPageBlockReference)
        {
            Key = key;
            ChildPageBlockReference = childPageBlockReference;
        }

        public static INBTEntry OfValue(NID nid, BREF bref)
            => new INBTEntry(nid, bref);
    }
}
