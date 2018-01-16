using pst.encodables.ndb;
using pst.encodables.ndb.btree;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.ndb
{
    class INBTEntryDecoder : IDecoder<INBTEntry>
    {
        private readonly IDecoder<BREF> brefDecoder;

        public INBTEntryDecoder(IDecoder<BREF> brefDecoder)
        {
            this.brefDecoder = brefDecoder;
        }

        public INBTEntry Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                INBTEntry.OfValue(
                    NID.OfValue(parser.TakeAndSkip(4)),
                    parser.TakeAtWithoutChangingStreamPosition(8, 16, brefDecoder));
        }
    }
}
