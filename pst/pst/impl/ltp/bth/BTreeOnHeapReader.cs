using pst.core;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ltp.bth;
using pst.interfaces.ltp.hn;
using pst.utilities;
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

        public Maybe<DataRecord> ReadDataRecord(BID blockId, TKey key)
        {
            var hnHeader = heapOnNodeReader.GetHeapOnNodeHeader(blockId);

            return ReadDataRecord(blockId, hnHeader.UserRoot, key);
        }

        public Maybe<DataRecord> ReadDataRecord(BID blockId, HID userRoot, TKey key)
        {
            var userRootHeapItem = heapOnNodeReader.GetHeapItem(blockId, userRoot);

            var bthHeader =
                bthHeaderDecoder.Decode(userRootHeapItem);

            if (bthHeader.Root.Value == 0)
                return Maybe<DataRecord>.NoValue();

            return
                FindDataRecord(
                    blockId,
                    bthHeader.Root,
                    key,
                    bthHeader.Key,
                    bthHeader.SizeOfDataValue,
                    bthHeader.IndexDepth);
        }

        public DataRecord[] ReadAllDataRecords(BID blockId, Maybe<HID> userRoot)
        {
            BTHHEADER bthHeader;

            if (userRoot.HasValue)
            {
                bthHeader =
                    bthHeaderDecoder
                    .Decode(heapOnNodeReader.GetHeapItem(blockId, userRoot.Value));
            }
            else
            {
                var hnHeader =
                    heapOnNodeReader.GetHeapOnNodeHeader(blockId);

                bthHeader =
                    bthHeaderDecoder
                    .Decode(heapOnNodeReader.GetHeapItem(blockId, hnHeader.UserRoot));
            }

            if (bthHeader.Root.Value == 0)
                return new DataRecord[0];

            var dataRecords = new List<DataRecord>();

            Enumerate(
                blockId,
                bthHeader.Root,
                bthHeader.Key,
                bthHeader.SizeOfDataValue,
                bthHeader.IndexDepth,
                dataRecords);

            return dataRecords.ToArray();
        }

        private void Enumerate(
            BID blockId,
            HID nodeId,
            int keySize,
            int dataSize,
            int currentDepth,
            List<DataRecord> dataRecords)
        {
            var node =
                heapOnNodeReader.GetHeapItem(blockId, nodeId);

            if (currentDepth > 0)
            {
                var parser = BinaryDataParser.OfValue(node);

                var itemCount = node.Length / (keySize + 4);

                for (var i = 0; i < itemCount; i++)
                {
                    var key = parser.TakeAndSkip(keySize);
                    var hid = parser.TakeAndSkip(4, hidDecoder);

                    Enumerate(
                        blockId,
                        hid,
                        keySize,
                        dataSize,
                        currentDepth - 1,
                        dataRecords);
                }
            }
            else
            {
                var parser = BinaryDataParser.OfValue(node);

                var itemCount = node.Length / (keySize + dataSize);

                for (var i = 0; i < itemCount; i++)
                {
                    var key = parser.TakeAndSkip(keySize);
                    var data = parser.TakeAndSkip(dataSize);

                    dataRecords.Add(new DataRecord(key, data));
                }
            }
        }

        private Maybe<DataRecord> FindDataRecord(
            BID blockId,
            HID nodeId,
            TKey keyToFind,
            int bthKeySize,
            int bthDataSize,
            int currentDepth)
        {
            var node =
                heapOnNodeReader.GetHeapItem(blockId, nodeId);

            if (currentDepth > 0)
            {
                var parser = BinaryDataParser.OfValue(node);

                var itemCount = node.Length / (bthKeySize + 4);

                var previousIndexRecord = (IndexRecord)null;

                for (var i = 0; i < itemCount; i++)
                {
                    var key = parser.TakeAndSkip(bthKeySize);
                    var hid = parser.TakeAndSkip(4, hidDecoder);

                    if (keyToFind.CompareTo(keyDecoder.Decode(key)) < 0)
                    {
                        return
                            FindDataRecord(
                                blockId,
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
                            blockId,
                            nodeId,
                            keyToFind,
                            bthKeySize,
                            bthDataSize,
                            currentDepth - 1);
                }
            }
            else
            {
                var parser = BinaryDataParser.OfValue(node);

                var itemCount = node.Length / (bthKeySize + bthDataSize);

                for (var i = 0; i < itemCount; i++)
                {
                    var key = parser.TakeAndSkip(bthKeySize);

                    var data = parser.TakeAndSkip(bthDataSize);

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
