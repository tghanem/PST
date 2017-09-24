using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDFLDModDel
    {
        //4 bytes
        public NID nidFld { get; }

        //4 bytes
        public int dwReserved { get; }

        public SUDFLDModDel(NID nidFld, int dwReserved)
        {
            this.nidFld = nidFld;
            this.dwReserved = dwReserved;
        }
    }
}
