using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging
{
    class PropertyTypeDecoder : IDecoder<PropertyType>
    {
        private readonly IDecoder<int> int32Decoder;

        public PropertyTypeDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public PropertyType Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            return new PropertyType(parser.TakeAndSkip(2, int32Decoder));
        }
    }
}
