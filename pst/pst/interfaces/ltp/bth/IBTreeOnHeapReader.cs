using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces.io;
using System;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapReader<TKey> where TKey : IComparable<TKey>
    {
        Maybe<DataRecord> ReadDataRecord(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            TKey key);

        Maybe<DataRecord> ReadDataRecord(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            HID userRoot,
            TKey key);

        DataRecord[] ReadAllDataRecords(
            IDataBlockReader<LBBTEntry> reader,
            IMapper<BID, LBBTEntry> blockIdToEntryMapping,
            LBBTEntry blockEntry,
            Maybe<HID> userRoot);
    }
}
