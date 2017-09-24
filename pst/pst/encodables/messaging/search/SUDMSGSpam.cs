using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDMSGSpam
    {
        //4 bytes
        public NID nidParent { get; }

        //4 bytes
        public NID nidMsg { get; }

        public SUDMSGSpam(NID nidParent, NID nidMsg)
        {
            this.nidParent = nidParent;
            this.nidMsg = nidMsg;
        }
    }
}
