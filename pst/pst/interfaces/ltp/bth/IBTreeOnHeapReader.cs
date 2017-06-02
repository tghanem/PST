using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb.btree;
using System;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapReader<TKey> where TKey : IComparable<TKey>
    {
        Maybe<DataRecord> ReadDataRecord(LBBTEntry blockEntry, TKey key);

        Maybe<DataRecord> ReadDataRecord(LBBTEntry blockEntry, HID userRoot, TKey key);

        DataRecord[] ReadAllDataRecords(LBBTEntry blockEntry, Maybe<HID> userRoot);
    }
}
