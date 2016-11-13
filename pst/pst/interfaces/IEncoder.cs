using pst.utilities;

namespace pst.interfaces
{
    interface IEncoder<TType> where TType : class
    {
        BinaryData Encode(TType value);
    }
}
