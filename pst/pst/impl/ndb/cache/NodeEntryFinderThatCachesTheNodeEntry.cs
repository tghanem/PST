using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ndb;

namespace pst.impl.ndb.cache
{
    class NodeEntryFinderThatCachesTheNodeEntry : INodeEntryFinder
    {
        private readonly ICache<NID[], NodeEntry> cache;
        private readonly INodeEntryFinder actualNodeEntryFinder;

        public NodeEntryFinderThatCachesTheNodeEntry(
            ICache<NID[], NodeEntry> cache,
            INodeEntryFinder actualNodeEntryFinder)
        {
            this.cache = cache;
            this.actualNodeEntryFinder = actualNodeEntryFinder;
        }

        public Maybe<NodeEntry> GetEntry(NID[] nodePath)
        {
            return cache.GetOrAdd(nodePath, () => actualNodeEntryFinder.GetEntry(nodePath));
        }
    }
}
