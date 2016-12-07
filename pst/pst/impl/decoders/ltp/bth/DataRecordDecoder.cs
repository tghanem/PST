using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.bth;

namespace pst.impl.decoders.ltp.bth
{
    class DataRecordDecoder : IDecoder<DataRecord>
    {
        private readonly int keySize;

        private readonly int dataSize;

        public DataRecordDecoder(int keySize, int dataSize)
        {
            this.keySize = keySize;
            this.dataSize = dataSize;
        }

        public DataRecord Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new DataRecord(
                    parser.TakeAndSkip(keySize),
                    parser.TakeAndSkip(dataSize));
        }
    }
}
