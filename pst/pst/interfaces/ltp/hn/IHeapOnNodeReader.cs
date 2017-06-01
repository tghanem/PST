using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeReader
    {
        HNHDR GetHeapOnNodeHeader(
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);

        BinaryData GetHeapItem(
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            HID hid);
    }
}
