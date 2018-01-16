using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ndb
{
    class HeaderDecoder : IDecoder<Header>
    {
        private readonly IDecoder<Root> rootDecoder;

        public HeaderDecoder(IDecoder<Root> rootDecoder)
        {
            this.rootDecoder = rootDecoder;
        }

        public Header Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new Header(
                    parser.TakeAndSkip(4),
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(2),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(4),
                    parser.TakeAndSkip(4),
                    parser.TakeAndSkip(8),
                    parser.TakeAndSkip(8).ToInt64(),
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(32, 4, new FuncBasedDecoder<NID>(NID.OfValue)),
                    parser.TakeAndSkip(8),
                    parser.TakeAndSkip(72, rootDecoder),
                    parser.TakeAndSkip(4),
                    parser.TakeAndSkip(128),
                    parser.TakeAndSkip(128),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(2),
                    parser.TakeAndSkip(8).ToInt64(),
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(3),
                    parser.TakeAndSkip(1),
                    parser.TakeAndSkip(32));
        }
    }
}
