using pst.interfaces.ltp.bth;
using System.Collections.Generic;
using pst.encodables.ltp.bth;
using pst.interfaces;
using pst.interfaces.ltp.hn;
using pst.encodables.ltp.hn;
using pst.utilities;

namespace pst.impl.ltp.bth
{
    class BTreeOnHeapLeafKeysEnumerator
        : IBTreeOnHeapLeafKeysEnumerator
    {
        private readonly IHeapOnNodeLoader heapOnNodeLoader;
        private readonly IDecoder<BTHHEADER> bthHeaderDecoder;
        private readonly IDecoder<HID> hidDecoder;

        public BTreeOnHeapLeafKeysEnumerator(
            IHeapOnNodeLoader heapOnNodeLoader,
            IDecoder<BTHHEADER> bthHeaderDecoder,
            IDecoder<HID> hidDecoder)
        {
            this.heapOnNodeLoader = heapOnNodeLoader;
            this.bthHeaderDecoder = bthHeaderDecoder;
            this.hidDecoder = hidDecoder;
        }

        public DataRecord[] Enumerate(HeapOnNode heapOnNode)
        {
            var bthHeader =
                bthHeaderDecoder.Decode(heapOnNode.Root);

            if (bthHeader.Root.Value == 0)
                return new DataRecord[0];

            var dataRecords = new List<DataRecord>();

            Enumerate(
                heapOnNode,
                bthHeader.Root,
                bthHeader.Key,
                bthHeader.SizeOfDataValue,
                bthHeader.IndexDepth,
                dataRecords);

            return dataRecords.ToArray();
        }

        public void Enumerate(HeapOnNode heapOnNode, HID nodeId, int keySize, int dataSize, int currentDepth, List<DataRecord> dataRecords)
        {
            var node = heapOnNode.GetItem(nodeId);

            if (currentDepth > 0)
            {
                var parser = BinaryDataParser.OfValue(node);

                var itemCount =
                    node.Length / (keySize + 4);

                for(var i = 0; i < itemCount; i++)
                {
                    var key = parser.TakeAndSkip(keySize);
                    var hid = parser.TakeAndSkip(4, hidDecoder);

                    Enumerate(
                        heapOnNode,
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

                var itemCount =
                    node.Length / (keySize + dataSize);

                for (var i = 0; i < itemCount; i++)
                {
                    var key = parser.TakeAndSkip(keySize);
                    var data = parser.TakeAndSkip(dataSize);

                    dataRecords.Add(
                        new DataRecord(key, data));
                }
            }
        }
    }
}
