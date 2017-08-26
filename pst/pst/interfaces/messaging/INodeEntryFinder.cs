using pst.core;

namespace pst.interfaces.messaging
{
    interface INodeEntryFinder
    {
        Maybe<NodeEntry> GetEntry(NodePath nodePath);
    }
}