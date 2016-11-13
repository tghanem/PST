namespace pst.encodables
{
    class INBTEntry
    {
        public NID NID { get; }

        public BREF BREF { get; }

        private INBTEntry(NID nid, BREF bref)
        {
            NID = nid;
            BREF = bref;
        }

        public static INBTEntry OfValue(NID nid, BREF bref)
            => new INBTEntry(nid, bref);
    }
}
