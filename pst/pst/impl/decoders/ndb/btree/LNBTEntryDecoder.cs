using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ndb
{
    class LNBTEntryDecoder : IDecoder<LNBTEntry>
    {
        private readonly IDecoder<BID> bidDecoder;

        public LNBTEntryDecoder(IDecoder<BID> bidDecoder)
        {
            this.bidDecoder = bidDecoder;
        }

        public LNBTEntry Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var nid = NID.OfValue(parser.TakeAndSkip(4));

            parser.TakeAndSkip(4);

            return
                new LNBTEntry(
                    nid,
                    parser.TakeAndSkip(8, bidDecoder),
                    parser.TakeAndSkip(8, bidDecoder),
                    NID.OfValue(parser.TakeAndSkip(4)),
                    parser.TakeAndSkip(4));
        }
    }
}
