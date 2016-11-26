using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.impl.decoders.ndb
{
    class LBBTEntryDecoder : IDecoder<LBBTEntry>
    {
        private readonly IDecoder<BREF> brefDecoder;

        private readonly IDecoder<int> int32Decoder;

        public LBBTEntryDecoder(IDecoder<BREF> brefDecoder, IDecoder<int> int32Decoder)
        {
            this.brefDecoder = brefDecoder;
            this.int32Decoder = int32Decoder;
        }

        public LBBTEntry Decode(BinaryData encodedData)
        {
            using (var parser = BinaryDataParser.OfValue(encodedData))
            {
                return
                    new LBBTEntry(
                        parser.TakeAndSkip(16, brefDecoder),
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(2, int32Decoder),
                        parser.TakeAndSkip(4));
            }
        }
    }
}
