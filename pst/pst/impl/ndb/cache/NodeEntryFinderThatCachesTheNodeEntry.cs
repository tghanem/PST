using pst.core;
using pst.interfaces;
using pst.interfaces.ndb;

namespace pst.impl.ndb.cache
{
    class NodeEntryFinderThatCachesTheNodeEntry : INodeEntryFinder
    {
        private readonly ICache<NodePath, NodeEntry> cache;
        private readonly INodeEntryFinder actualNodeEntryFinder;

        public NodeEntryFinderThatCachesTheNodeEntry(
            ICache<NodePath, NodeEntry> cache,
            INodeEntryFinder actualNodeEntryFinder)
        {
            this.cache = cache;
            this.actualNodeEntryFinder = actualNodeEntryFinder;
        }

        public Maybe<NodeEntry> GetEntry(NodePath nodePath)
        {
            return cache.GetOrAdd(nodePath, () => actualNodeEntryFinder.GetEntry(nodePath));
        }
    }
}
