using pst.core;
using pst.interfaces.io;

namespace pst.interfaces.btree
{
    interface IBTreeNodeLoader<TNode, TNodeReference>
        where TNodeReference : class
        where TNode : class
    {
        Maybe<TNode> LoadNode(IDataBlockReader<TNodeReference> reader, TNodeReference nodeReference);
    }
}
