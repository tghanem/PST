using pst.utilities;

namespace pst.interfaces
{
    interface IDecoder<TType> where TType : class
    {
        TType Decode(BinaryData encodedData);
    }
}
