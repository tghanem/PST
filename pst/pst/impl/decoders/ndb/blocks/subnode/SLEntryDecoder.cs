using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb;

namespace pst.impl.decoders.ndb.blocks.subnode
{
    class SLEntryDecoder : IDecoder<SLEntry>
    {
        private readonly IDecoder<NID> nidDecoder;
        private readonly IDecoder<BID> bidDecoder;

        public SLEntryDecoder(IDecoder<NID> nidDecoder, IDecoder<BID> bidDecoder)
        {
            this.nidDecoder = nidDecoder;
            this.bidDecoder = bidDecoder;
        }

        public SLEntry Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var localNID = parser.TakeAndSkip(8);

            var subnodeBID = parser.TakeAndSkip(8);

            var subnodeNID = parser.TakeAndSkip(8);

            return
                new SLEntry(
                    nidDecoder.Decode(localNID.Take(4)),
                    bidDecoder.Decode(subnodeBID),
                    bidDecoder.Decode(subnodeNID));
        }
    }
}
