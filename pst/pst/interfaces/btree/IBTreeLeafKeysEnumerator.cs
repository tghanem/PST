namespace pst.interfaces.btree
{
    interface IBTreeLeafKeysEnumerator<TKey, TNodeReference>
    {
        TKey[] Enumerate(TNodeReference rootNodeReference);
    }
}
