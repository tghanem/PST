using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.decoders.ndb
{
    class HeaderDecoder : IDecoder<Header>
    {
        private readonly IDecoder<int> int32Decoder;

        private readonly IDecoder<Root> rootDecoder;

        private readonly IDecoder<BID> bidDecoder;

        private readonly IDecoder<NID> nidDecoder;

        public HeaderDecoder(IDecoder<int> int32Decoder, IDecoder<Root> rootDecoder, IDecoder<BID> bidDecoder, IDecoder<NID> nidDecoder)
        {
            this.int32Decoder = int32Decoder;
            this.rootDecoder = rootDecoder;
            this.bidDecoder = bidDecoder;
            this.nidDecoder = nidDecoder;
        }

        public Header Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new Header(
                        parser.TakeAndSkip(4),
                        parser.TakeAndSkip(4, int32Decoder),
                        parser.TakeAndSkip(2),
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(4),
                        parser.TakeAndSkip(4),
                        parser.TakeAndSkip(8),
                        parser.TakeAndSkip(8, bidDecoder),
                        parser.TakeAndSkip(4, int32Decoder),
                        parser.TakeAndSkip(32, 4, nidDecoder),
                        parser.TakeAndSkip(8),
                        parser.TakeAndSkip(72, rootDecoder),
                        parser.TakeAndSkip(4),
                        parser.TakeAndSkip(128),
                        parser.TakeAndSkip(128),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(1, int32Decoder),
                        parser.TakeAndSkip(2),
                        parser.TakeAndSkip(8, bidDecoder),
                        parser.TakeAndSkip(4, int32Decoder),
                        parser.TakeAndSkip(3),
                        parser.TakeAndSkip(1),
                        parser.TakeAndSkip(32));
            }
        }
    }
}
