namespace pst.interfaces
{
    interface IMapper<TInput, TOutput>
    {
        TOutput Map(TInput input);
    }
}
