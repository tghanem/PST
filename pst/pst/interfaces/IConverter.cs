namespace pst.interfaces
{
    interface IConverter<TInput, TOutput>
    {
        TOutput Convert(TInput parameter);
    }
}
