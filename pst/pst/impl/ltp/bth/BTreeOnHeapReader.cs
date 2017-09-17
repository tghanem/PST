using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.interfaces.ndb;
using System;
using System.Collections.Generic;

namespace pst.impl.ltp.bth
{
    class BTreeOnHeapReader<TKey> : IBTreeOnHeapReader<TKey> where TKey : IComparable<TKey>
    {
        private readonly IDecoder<HID> hidDecoder;
        private readonly IDecoder<TKey> keyDecoder;
        private readonly IDecoder<BTHHEADER> bthHeaderDecoder;
        private readonly IHeapOnNodeReader heapOnNodeReader;

        public BTreeOnHeapReader(
            IDecoder<HID> hidDecoder,
            IDecoder<TKey> keyDecoder,
            IDecoder<BTHHEADER> bthHeaderDecoder,
            IHeapOnNodeReader heapOnNodeReader)
        {
            this.keyDecoder = keyDecoder;
            this.hidDecoder = hidDecoder;
            this.heapOnNodeReader = heapOnNodeReader;
            this.bthHeaderDecoder = bthHeaderDecoder;
        }

        public Maybe<DataRecord> ReadDataRecord(NodePath nodePath, TKey key)
        {
            var hnHeader = heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

            return ReadDataRecord(nodePath, hnHeader.UserRoot, key);
        }

        public Maybe<DataRecord> ReadDataRecord(NodePath nodePath, HID userRoot, TKey key)
        {
            var userRootHeapItem = heapOnNodeReader.GetHeapItem(nodePath, userRoot);

            var bthHeader =
                bthHeaderDecoder.Decode(userRootHeapItem);

            if (bthHeader.Root.Value == 0)
                return Maybe<DataRecord>.NoValue();

            return
                FindDataRecord(
                    nodePath,
                    bthHeader.Root,
                    key,
                    bthHeader.Key,
                    bthHeader.SizeOfDataValue,
                    bthHeader.IndexDepth);
        }

        public DataRecord[] ReadAllDataRecords(NodePath nodePath, Maybe<HID> userRoot)
        {
            BTHHEADER bthHeader;

            if (userRoot.HasValue)
            {
                bthHeader =
                    bthHeaderDecoder
                    .Decode(heapOnNodeReader.GetHeapItem(nodePath, userRoot.Value));
            }
            else
            {
                var hnHeader =
                    heapOnNodeReader.GetHeapOnNodeHeader(nodePath);

                bthHeader =
                    bthHeaderDecoder
                    .Decode(heapOnNodeReader.GetHeapItem(nodePath, hnHeader.UserRoot));
            }

            if (bthHeader.Root.Value == 0)
                return new DataRecord[0];

            var dataRecords = new List<DataRecord>();

            Enumerate(
                nodePath,
                bthHeader.Root,
                bthHeader.Key,
                bthHeader.SizeOfDataValue,
                bthHeader.IndexDepth,
                dataRecords);

            return dataRecords.ToArray();
        }

        private void Enumerate(
            NodePath nodePath,
            HID nodeId,
            int keySize,
            int dataSize,
            int currentDepth,
            List<DataRecord> dataRecords)
        {
            var node =
                heapOnNodeReader.GetHeapItem(nodePath, nodeId);

            if (currentDepth > 0)
            {
                var items = node.Slice(keySize + 4);

                Array.ForEach(
                    items,
                    item =>
                    {
                        var hid = hidDecoder.Decode(item.Take(keySize, 4));

                        Enumerate(
                            nodePath,
                            hid,
                            keySize,
                            dataSize,
                            currentDepth - 1,
                            dataRecords);
                    });
            }
            else
            {
                var items = node.Slice(keySize + dataSize);

                Array.ForEach(
                    items,
                    item =>
                    {
                        dataRecords.Add(
                            new DataRecord(
                                item.Take(keySize),
                                item.Take(keySize, dataSize)));
                    });
            }
        }

        private Maybe<DataRecord> FindDataRecord(
            NodePath nodePath,
            HID nodeId,
            TKey keyToFind,
            int bthKeySize,
            int bthDataSize,
            int currentDepth)
        {
            var node =
                heapOnNodeReader.GetHeapItem(nodePath, nodeId);

            if (currentDepth > 0)
            {
                var items = node.Slice(bthKeySize + 4);

                var previousIndexRecord = (IndexRecord)null;

                for (var i = 0; i < items.Length; i++)
                {
                    var key = items[i].Take(bthKeySize);
                    var hid = hidDecoder.Decode(items[i].Take(bthKeySize, 4));

                    if (keyToFind.CompareTo(keyDecoder.Decode(key)) < 0)
                    {
                        return
                            FindDataRecord(
                                nodePath,
                                nodeId,
                                keyToFind,
                                bthKeySize,
                                bthDataSize,
                                currentDepth - 1);
                    }

                    previousIndexRecord = new IndexRecord(key, hid);
                }

                if (keyToFind.CompareTo(keyDecoder.Decode(previousIndexRecord.Key)) > 0)
                {
                    return
                        FindDataRecord(
                            nodePath,
                            nodeId,
                            keyToFind,
                            bthKeySize,
                            bthDataSize,
                            currentDepth - 1);
                }
            }
            else
            {
                var items = node.Slice(bthKeySize + bthDataSize);

                for (var i = 0; i < items.Length; i++)
                {
                    var key = items[i].Take(bthKeySize);
                    var data = items[i].Take(bthKeySize, bthDataSize);

                    var decodedKey = keyDecoder.Decode(key);

                    if (keyToFind.CompareTo(decodedKey) == 0)
                    {
                        return Maybe<DataRecord>.OfValue(new DataRecord(key, data));
                    }
                }
            }

            return Maybe<DataRecord>.NoValue();
        }
    }
}
