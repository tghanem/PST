using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.decoders.ndb
{
    class LNBTEntryDecoder : IDecoder<LNBTEntry>
    {
        private readonly IDecoder<NID> nidDecoder;

        private readonly IDecoder<BID> bidDecoder;

        public LNBTEntryDecoder(IDecoder<NID> nidDecoder, IDecoder<BID> bidDecoder)
        {
            this.nidDecoder = nidDecoder;
            this.bidDecoder = bidDecoder;
        }

        public LNBTEntry Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var nid = parser.TakeAndSkip(4, nidDecoder);

            parser.TakeAndSkip(4);

            return
                new LNBTEntry(
                    nid,
                    parser.TakeAndSkip(8, bidDecoder),
                    parser.TakeAndSkip(8, bidDecoder),
                    parser.TakeAndSkip(4, nidDecoder),
                    parser.TakeAndSkip(4));
        }
    }
}
