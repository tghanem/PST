using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDMSGIDX
    {
        //4 bytes
        public NID nidMsg { get; }

        public SUDMSGIDX(NID nidMsg)
        {
            this.nidMsg = nidMsg;
        }
    }
}
