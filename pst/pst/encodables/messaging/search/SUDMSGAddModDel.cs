using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDMSGAddModDel
    {
        //4 bytes
        public NID nidParent { get; }

        //4 bytes
        public NID nidMsg { get; }

        public SUDMSGAddModDel(NID nidParent, NID nidMsg)
        {
            this.nidParent = nidParent;
            this.nidMsg = nidMsg;
        }
    }
}
