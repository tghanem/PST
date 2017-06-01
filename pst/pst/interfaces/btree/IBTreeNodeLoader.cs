namespace pst.interfaces.btree
{
    interface IBTreeNodeLoader<TNode, TNodeReference>
        where TNodeReference : class
        where TNode : class
    {
        TNode LoadNode(TNodeReference nodeReference);
    }
}
