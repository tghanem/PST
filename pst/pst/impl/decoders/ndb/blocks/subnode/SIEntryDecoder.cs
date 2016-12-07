using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks.subnode;
using pst.encodables.ndb;

namespace pst.impl.decoders.ndb.blocks.subnode
{
    class SIEntryDecoder : IDecoder<SIEntry>
    {
        private readonly IDecoder<NID> nidDecoder;

        private readonly IDecoder<BID> bidDecoder;

        public SIEntryDecoder(IDecoder<NID> nidDecoder, IDecoder<BID> bidDecoder)
        {
            this.nidDecoder = nidDecoder;
            this.bidDecoder = bidDecoder;
        }

        public SIEntry Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new SIEntry(
                    parser.TakeAndSkip(8, nidDecoder),
                    parser.TakeAndSkip(8, bidDecoder));
        }
    }
}
