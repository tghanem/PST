namespace pst.interfaces
{
    interface IFactory<TType>
    {
        TType Create();
    }
}