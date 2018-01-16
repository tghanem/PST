using pst.encodables;
using pst.encodables.ndb;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders
{
    class EntryIdDecoder : IDecoder<EntryId>
    {
        public EntryId Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return
                new EntryId(
                    parser.TakeAndSkip(4).ToInt32(),
                    parser.TakeAndSkip(16).Value,
                    NID.OfValue(parser.TakeAndSkip(4)));
        }
    }
}
