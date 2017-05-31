using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeReader
    {
        HNHDR GetHeapOnNodeHeader(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry);

        BinaryData GetHeapItem(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            HID hid);
    }
}
