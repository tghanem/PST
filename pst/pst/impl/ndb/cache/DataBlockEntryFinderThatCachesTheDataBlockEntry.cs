using pst.core;
using pst.encodables.ndb;
using pst.interfaces;
using pst.interfaces.ndb;

namespace pst.impl.ndb.cache
{
    class DataBlockEntryFinderThatCachesTheDataBlockEntry : IDataBlockEntryFinder
    {
        private readonly IDataBlockEntryFinder actualDataBlockEntryFinder;
        private readonly ICache<BID, DataBlockEntry> cache;

        public DataBlockEntryFinderThatCachesTheDataBlockEntry(
            IDataBlockEntryFinder actualDataBlockEntryFinder,
            ICache<BID, DataBlockEntry> cache)
        {
            this.actualDataBlockEntryFinder = actualDataBlockEntryFinder;
            this.cache = cache;
        }

        public Maybe<DataBlockEntry> Find(BID blockId)
        {
            return cache.GetOrAdd(blockId, () => actualDataBlockEntryFinder.Find(blockId));
        }
    }
}
