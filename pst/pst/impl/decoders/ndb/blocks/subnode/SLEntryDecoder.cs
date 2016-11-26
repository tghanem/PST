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
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new SLEntry(
                        parser.TakeAndSkip(8, nidDecoder),
                        parser.TakeAndSkip(8, bidDecoder),
                        parser.TakeAndSkip(8, nidDecoder));
            }
        }
    }
}
