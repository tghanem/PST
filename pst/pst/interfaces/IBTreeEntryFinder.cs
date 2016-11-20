namespace pst.interfaces
{
    interface IBTreeEntryFinder<TKey, TIntermediateEntry, TLeafEntry>
    {
         TLeafEntry Find(TKey key);
    }
}