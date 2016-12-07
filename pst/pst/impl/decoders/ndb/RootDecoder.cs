using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.decoders.ndb
{
    class RootDecoder : IDecoder<Root>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<long> int64Decoder;

        private readonly IDecoder<BREF> brefDecoder;

        public RootDecoder(IDecoder<int> int32Decoder, IDecoder<long> int64Decoder, IDecoder<BREF> brefDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.int64Decoder = int64Decoder;
            this.brefDecoder = brefDecoder;
        }

        public Root Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new Root(
                    parser.TakeAndSkip(4, int32Decoder),
                    parser.TakeAndSkip(8, int64Decoder),
                    parser.TakeAndSkip(8, int64Decoder),
                    parser.TakeAndSkip(8, int64Decoder),
                    parser.TakeAndSkip(8, int64Decoder),
                    parser.TakeAndSkip(16, brefDecoder),
                    parser.TakeAndSkip(16, brefDecoder),
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(1, int32Decoder),
                    parser.TakeAndSkip(2, int32Decoder));
        }
    }
}
