using pst.utilities;

namespace pst.interfaces
{
    interface IEncoder<TType>
    {
        BinaryData Encode(TType value);
    }
}
