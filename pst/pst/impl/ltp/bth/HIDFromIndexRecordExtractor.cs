using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.interfaces;

namespace pst.impl.ltp.bth
{
    class HIDFromIndexRecordExtractor : IExtractor<IndexRecord, HID>
    {
        public HID Extract(IndexRecord parameter)
        {
            return parameter.NextLevelId;
        }
    }
}
