using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;

namespace pst.impl.decoders.ndb
{
    class IBBTEntryDecoder : IDecoder<IBBTEntry>
    {
        private readonly IDecoder<BID> bidDecoder;

        private readonly IDecoder<BREF> brefDecoder;

        public IBBTEntryDecoder(IDecoder<BID> bidDecoder, IDecoder<BREF> brefDecoder)
        {
            this.bidDecoder = bidDecoder;
            this.brefDecoder = brefDecoder;
        }

        public IBBTEntry Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    IBBTEntry.OfValue(
                        parser.TakeAndSkip(8, bidDecoder),
                        parser.TakeAndSkip(16, brefDecoder));
            }
        }
    }
}
