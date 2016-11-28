using pst.encodables.ltp.bth;
using pst.interfaces;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ltp.bth
{
    class DataRecordsFromBTreeOnHeapNodeExtractor : IExtractor<BTreeOnHeapNode, DataRecord[]>
    {
        private readonly IDecoder<DataRecord> dataRecordDecoder;

        private readonly int keySize;

        private readonly int dataSize;

        public DataRecordsFromBTreeOnHeapNodeExtractor(IDecoder<DataRecord> dataRecordDecoder, int keySize, int dataSize)
        {
            this.dataRecordDecoder = dataRecordDecoder;
            this.keySize = keySize;
            this.dataSize = dataSize;
        }

        public DataRecord[] Extract(BTreeOnHeapNode parameter)
        {
            var records = new List<DataRecord>();

            using (var parser = BinaryDataParser.OfValue(parameter.Records))
            {
                var numberOfRecords =
                    parameter.Records.Length / (keySize + dataSize);

                for (int i = 0; i < numberOfRecords; i++)
                {
                    records.Add(parser.TakeAndSkip(keySize + dataSize, dataRecordDecoder));
                }
            }

            return records.ToArray();
        }
    }
}
