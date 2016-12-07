using pst.encodables.ltp.hn;
using pst.utilities;

namespace pst.encodables.ltp.bth
{
    class IndexRecord
    {
        ///(variable)
        public BinaryData Key { get; }

        ///4
        public HID NextLevelId { get; }

        public IndexRecord(BinaryData key, HID nextLevelId)
        {
            Key = key;
            NextLevelId = nextLevelId;
        }
    }
}
