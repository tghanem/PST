using pst.encodables.ltp.bth;
using pst.interfaces;
using pst.utilities;
using System.Collections.Generic;

namespace pst.impl.ltp.bth
{
    class IndexRecordsFromBTreeOnHeapNodeExtractor : IExtractor<BTreeOnHeapNode, IndexRecord[]>
    {
        private readonly IDecoder<IndexRecord> indexRecordDecoder;

        private readonly int keySize;

        public IndexRecordsFromBTreeOnHeapNodeExtractor(IDecoder<IndexRecord> indexRecordDecoder, int keySize)
        {
            this.indexRecordDecoder = indexRecordDecoder;
            this.keySize = keySize;
        }

        public IndexRecord[] Extract(BTreeOnHeapNode parameter)
        {
            var records = new List<IndexRecord>();

            using (var parser = BinaryDataParser.OfValue(parameter.Records))
            {
                var numberOfRecords =
                    parameter.Records.Length / (keySize + 4);

                for(int i = 0; i < numberOfRecords; i++)
                {
                    records.Add(parser.TakeAndSkip(keySize + 4, indexRecordDecoder));
                }
            }

            return records.ToArray();
        }
    }
}
