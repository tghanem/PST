using pst.utilities;

namespace pst.interfaces
{
    interface IDecoder<TType>
    {
        TType Decode(BinaryData encodedData);
    }
}
