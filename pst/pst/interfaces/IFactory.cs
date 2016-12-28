namespace pst.interfaces
{
    interface IFactory<TInput, TOutput>
    {
        TOutput Create(TInput input);
    }
}
