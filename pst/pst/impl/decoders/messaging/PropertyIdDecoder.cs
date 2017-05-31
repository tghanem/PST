using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging
{
    class PropertyIdDecoder : IDecoder<PropertyId>
    {
        private readonly IDecoder<int> int32Decoder;

        public PropertyIdDecoder(IDecoder<int> int32Decoder)
        {
            this.int32Decoder = int32Decoder;
        }

        public PropertyId Decode(BinaryData encodedData)
        {
            return new PropertyId(int32Decoder.Decode(encodedData.Take(4)));
        }
    }
}
