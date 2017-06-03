using pst.encodables.ndb;

namespace pst.encodables
{
    class EntryId
    {
        public int Flags { get; }

        public byte[] UID { get; }

        public NID NID { get; }

        public EntryId(int flags, byte[] uid, NID nid)
        {
            Flags = flags;
            UID = uid;
            NID = nid;
        }
    }
}
