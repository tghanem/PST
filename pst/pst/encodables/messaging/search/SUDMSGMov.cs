using pst.encodables.ndb;

namespace pst.encodables.messaging.search
{
    class SUDMSGMov
    {
        public NID nidParentNew { get; }

        public NID nidMsg { get; }

        public NID nidParentOld { get; }

        public SUDMSGMov(NID nidParentNew, NID nidMsg, NID nidParentOld)
        {
            this.nidParentNew = nidParentNew;
            this.nidMsg = nidMsg;
            this.nidParentOld = nidParentOld;
        }
    }
}
