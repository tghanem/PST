namespace pst.interfaces
{
    public interface IDecoder<TType> where TType : class
    {
        TType Decode(BinaryData encodedData);
    }
}
