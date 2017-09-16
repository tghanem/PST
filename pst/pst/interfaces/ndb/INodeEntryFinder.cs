using pst.core;

namespace pst.interfaces.ndb
{
    interface INodeEntryFinder
    {
        Maybe<NodeEntry> GetEntry(NodePath nodePath);
    }
}