using pst.interfaces;
using pst.utilities;
using pst.encodables.ltp.bth;
using pst.encodables.ltp.hn;

namespace pst.impl.decoders.ltp.bth
{
    class IndexRecordDecoder : IDecoder<IndexRecord>
    {
        private readonly IDecoder<HID> hidDecoder;

        private readonly int keySize;

        public IndexRecordDecoder(IDecoder<HID> hidDecoder, int keySize)
        {
            this.hidDecoder = hidDecoder;
            this.keySize = keySize;
        }

        public IndexRecord Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new IndexRecord(
                    parser.TakeAndSkip(keySize),
                    parser.TakeAndSkip(4, hidDecoder));
        }
    }
}
