using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeReader
    {
        HNHDR GetHeapOnNodeHeader(BID blockId);

        BinaryData GetHeapItem(BID blockId, HID hid);
    }
}
