using pst.encodables.ltp.hn;
using pst.interfaces.ndb;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeReader
    {
        HNHDR GetHeapOnNodeHeader(NodePath nodePath);

        BinaryData GetHeapItem(NodePath nodePath, HID hid);
    }
}
