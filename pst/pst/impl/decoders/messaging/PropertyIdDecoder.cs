using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging
{
    class PropertyIdDecoder : IDecoder<PropertyId>
    {
        public PropertyId Decode(BinaryData encodedData)
        {
            return new PropertyId(encodedData.Take(4).ToInt32());
        }
    }
}
