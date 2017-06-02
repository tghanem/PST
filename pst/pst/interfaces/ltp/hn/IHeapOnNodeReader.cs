using pst.encodables.ltp.hn;
using pst.encodables.ndb.btree;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeReader
    {
        HNHDR GetHeapOnNodeHeader(LBBTEntry blockEntry);

        BinaryData GetHeapItem(LBBTEntry blockEntry, HID hid);
    }
}
