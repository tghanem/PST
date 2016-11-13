namespace pst.encodables
{
    class IBBTEntry
    {
        public BID BID { get; }

        public BREF BREF { get; }

        private IBBTEntry(BID bid, BREF bref)
        {
            BID = bid;
            BREF = bref;
        }

        public static IBBTEntry OfValue(BID bid, BREF bref)
            => new IBBTEntry(bid, bref);
    }
}
