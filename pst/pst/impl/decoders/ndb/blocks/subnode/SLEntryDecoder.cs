using pst.encodables.ndb;
using pst.encodables.ndb.blocks.subnode;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ndb.blocks.subnode
{
    class SLEntryDecoder : IDecoder<SLEntry>
    {
        private readonly IDecoder<BID> bidDecoder;

        public SLEntryDecoder(IDecoder<BID> bidDecoder)
        {
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
                    NID.OfValue(localNID.Take(4)),
                    bidDecoder.Decode(subnodeBID),
                    bidDecoder.Decode(subnodeNID));
        }
    }
}
