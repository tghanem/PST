namespace pst.interfaces
{
    public interface IEncoder<TType> where TType : class
    {
        BinaryData Encode(TType value);
    }
}
