using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.decoders.ndb
{
    class RootDecoder : IDecoder<Root>
    {
        private readonly IDecoder<BREF> brefDecoder;

        public RootDecoder(IDecoder<BREF> brefDecoder)
        {
            this.brefDecoder = brefDecoder;
        }

        public Root Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new Root(
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(8).ToInt64(),
                    parser.TakeAndSkip(8).ToInt64(),
                    parser.TakeAndSkip(8).ToInt64(),
                    parser.TakeAndSkip(8).ToInt64(),
                    parser.TakeAndSkip(16, brefDecoder),
                    parser.TakeAndSkip(16, brefDecoder),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(1).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32());
        }
    }
}
