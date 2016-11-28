namespace pst.interfaces
{
    interface IExtractor<TInput, TOutput>
    {
        TOutput Extract(TInput parameter);
    }
}
