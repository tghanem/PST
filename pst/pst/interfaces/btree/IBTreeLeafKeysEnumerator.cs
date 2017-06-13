namespace pst.interfaces.btree
{
    interface IBTreeLeafKeysEnumerator<TKey, TNodeReference>
        where TNodeReference : class
        where TKey : class
    {
        TKey[] Enumerate(TNodeReference rootNodeReference);
    }
}
