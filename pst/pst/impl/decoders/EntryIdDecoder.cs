using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders
{
    class EntryIdDecoder : IDecoder<EntryId>
    {
        private readonly IDecoder<NID> nidDecoder;

        public EntryIdDecoder(IDecoder<NID> nidDecoder)
        {
            this.nidDecoder = nidDecoder;
        }

        public EntryId Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new EntryId(
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(16).Value,
                    parser.TakeAndSkip(4, nidDecoder));
        }
    }
}
