using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDFLDAddMov
    {
        //4 bytes
        public NID nidParent { get; }

        //4 bytes
        public NID nidMsg { get; }

        //4 bytes
        public int dwReserved1 { get; }

        //4 bytes
        public int dwReserved2 { get; }

        public SUDFLDAddMov(NID nidParent, NID nidMsg, int dwReserved1, int dwReserved2)
        {
            this.nidParent = nidParent;
            this.nidMsg = nidMsg;
            this.dwReserved1 = dwReserved1;
            this.dwReserved2 = dwReserved2;
        }
    }
}
