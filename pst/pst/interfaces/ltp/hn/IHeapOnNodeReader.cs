using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeReader
    {
        HNHDR GetHeapOnNodeHeader(NID[] nodePath);

        BinaryData GetHeapItem(NID[] nodePath, HID hid);
    }
}
