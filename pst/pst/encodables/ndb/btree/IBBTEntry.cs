namespace pst.encodables.ndb.btree
{
    class IBBTEntry
    {
        public BID Key { get; }

        public BREF ChildPageBlockReference { get; }

        private IBBTEntry(BID key, BREF childPageBlockReference)
        {
            Key = key;
            ChildPageBlockReference = childPageBlockReference;
        }

        public static IBBTEntry OfValue(BID bid, BREF bref)
            => new IBBTEntry(bid, bref);
    }
}
