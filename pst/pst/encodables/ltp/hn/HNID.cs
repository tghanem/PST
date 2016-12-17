using pst.core;
using pst.encodables.ndb;

namespace pst.encodables.ltp.hn
{
    class HNID
    {
        private Maybe<HID> hid;
        private Maybe<NID> nid;
        
        public HNID(HID hid, NID nid)
        {
            this.hid = hid;
            this.nid = nid;
        }

        public bool IsHID => hid.HasValue;
        public bool IsNID => nid.HasValue;

        public HID HID => hid.Value;
        public NID NID => nid.Value;

        public bool IsZero
            =>
            (hid.HasValue && hid.Value.IsZero) ||
            (nid.HasValue && nid.Value.IsZero);
    }
}
