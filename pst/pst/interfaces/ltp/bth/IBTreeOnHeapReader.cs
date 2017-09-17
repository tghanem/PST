using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.interfaces.ndb;
using System;

namespace pst.interfaces.ltp.bth
{
    interface IBTreeOnHeapReader<TKey> where TKey : IComparable<TKey>
    {
        Maybe<DataRecord> ReadDataRecord(NodePath nodePath, TKey key);

        Maybe<DataRecord> ReadDataRecord(NodePath nodePath, HID userRoot, TKey key);

        DataRecord[] ReadAllDataRecords(NodePath nodePath, Maybe<HID> userRoot);
    }
}
