using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders.messaging
{
    public class PropertyTagDecoder : IDecoder<PropertyTag>
    {
        public PropertyTag Decode(BinaryData encodedData)
        {
            var propertyId =
                new PropertyId(encodedData.Take(2).ToInt32());

            var propertyType =
                new PropertyType(encodedData.Take(2, 2).ToInt32());

            return new PropertyTag(propertyId, propertyType);
        }
    }
}
