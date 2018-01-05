using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using System;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapReader<TKey> where TKey : IComparable<TKey>
    {
        Maybe<DataRecord> ReadDataRecord(NID[] nodePath, TKey key);

        Maybe<DataRecord> ReadDataRecord(NID[] nodePath, HID userRoot, TKey key);

        DataRecord[] ReadAllDataRecords(NID[] nodePath, Maybe<HID> userRoot);
    }
}
