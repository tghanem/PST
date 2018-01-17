using pst.core;
using pst.encodables.ndb;
using pst.utilities;

namespace pst.encodables.ltp.hn
{
    class HNID
    {
        private Maybe<HID> hid;
        private Maybe<NID> nid;

        public HNID(HID hid)
        {
            this.hid = hid;
        }

        public HNID(NID nid)
        {
            this.nid = nid;
        }

        public static HNID OfValue(BinaryData encodedData)
        {
            var hid = HID.OfValue(encodedData);

            if (hid.Type == Constants.NID_TYPE_HID)
            {
                return new HNID(hid);
            }

            return new HNID(NID.OfValue(encodedData));
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
