using pst.interfaces;
using pst.utilities;
using pst.encodables;

namespace pst.impl.decoders
{
    class INBTEntryDecoder : IDecoder<INBTEntry>
    {
        private readonly IDecoder<NID> nidDecoder;

        private readonly IDecoder<BREF> brefDecoder;

        public INBTEntryDecoder(IDecoder<NID> nidDecoder, IDecoder<BREF> brefDecoder)
        {
            this.nidDecoder = nidDecoder;
            this.brefDecoder = brefDecoder;
        }

        public INBTEntry Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    INBTEntry.OfValue(
                        parser.TakeAndSkip(8, nidDecoder),
                        parser.TakeAndSkip(16, brefDecoder));
            }
        }
    }
}
