namespace pst.interfaces.btree
{
    interface IBTreeLeafKeysEnumerator<TKey, TNodeReference>
        where TNodeReference : class
        where TKey : class
    {
        TKey[] Enumerate(TNodeReference rootNodeReference);
    }

    interface IBTreeLeafKeysEnumeratorThatDoesntKnowHowToMapTheKeyToNodeReference<TLeafKey, TIntermediateKey, TNodeReference>
        where TIntermediateKey : class
        where TNodeReference : class
        where TLeafKey : class
    {
        TLeafKey[] Enumerate(TNodeReference rootNodeReference);
    }
}
