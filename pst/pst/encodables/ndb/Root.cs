namespace pst.encodables.ndb
{
    class Root
    {
        public int Reserved { get; }

        public long FileEOF { get; }

        public long AMapLast { get; }

        public long AMapFree { get; }

        public long PMapFree { get; }

        public BREF NBTRootPage { get; }

        public BREF BBTRootPage { get; }

        public int AMapValid { get; }

        public int BReserved { get; }

        public int WReserved { get; }

        public Root(
            int reserved,
            long fileEOF,
            long amapLast,
            long amapFree,
            long pmapFree,
            BREF nbtRootPage,
            BREF bbtRootPage,
            int amapValid,
            int bReserved,
            int wReserved)
        {
            Reserved = reserved;
            FileEOF = fileEOF;
            AMapLast = amapLast;
            AMapFree = amapFree;
            PMapFree = pmapFree;
            NBTRootPage = nbtRootPage;
            BBTRootPage = bbtRootPage;
            AMapValid = amapValid;
            BReserved = bReserved;
            WReserved = wReserved;
        }
    }
}
