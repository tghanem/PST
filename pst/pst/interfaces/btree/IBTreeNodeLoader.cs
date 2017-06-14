namespace pst.interfaces.btree
{
    interface IBTreeNodeLoader<TNode, TNodeReference>
    {
        TNode LoadNode(TNodeReference nodeReference);
    }
}
