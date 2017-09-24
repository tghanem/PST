using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDSRCHAddDel
    {
        //4 bytes
        public NID nidSrch { get; }

        public SUDSRCHAddDel(NID nidSrch)
        {
            this.nidSrch = nidSrch;
        }
    }
}
