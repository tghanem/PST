using pst.encodables;
using pst.interfaces;
using pst.utilities;

namespace pst.impl.decoders
{
    class TagDecoder : IDecoder<Tag>
    {
        public Tag Decode(BinaryData encodedData)
        {
            return Tag.OfValue(encodedData);
        }
    }
}
