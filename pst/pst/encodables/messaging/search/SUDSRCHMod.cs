using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDSRCHMod
    {
        //4 bytes
        public NID nidSrch { get; }

        //4 bytes
        public int dwReserved { get; }

        public SUDSRCHMod(NID nidSrch, int dwReserved)
        {
            this.nidSrch = nidSrch;
            this.dwReserved = dwReserved;
        }
    }
}
