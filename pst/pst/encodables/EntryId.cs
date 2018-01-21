using pst.encodables.ndb;
using pst.utilities;

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

        public static EntryId OfValue(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new EntryId(
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(16).Value,
                    NID.OfValue(parser.TakeAndSkip(4)));
        }
    }
}
