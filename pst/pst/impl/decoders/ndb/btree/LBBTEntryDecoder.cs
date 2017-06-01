using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.decoders.ndb
{
    class LBBTEntryDecoder : IDecoder<LBBTEntry>
    {
        private readonly IDecoder<BREF> brefDecoder;

        public LBBTEntryDecoder(IDecoder<BREF> brefDecoder)
        {
            this.brefDecoder = brefDecoder;
        }

        public LBBTEntry Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new LBBTEntry(
                    parser.TakeAndSkip(16, brefDecoder),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(2).ToInt32(),
                    parser.TakeAndSkip(4));
        }
    }
}
