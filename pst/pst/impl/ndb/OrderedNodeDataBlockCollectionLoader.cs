using pst.interfaces.ndb;
using pst.encodables.ndb;
using pst.interfaces.btree;
using pst.encodables.ndb.btree;
using pst.core;
using pst.interfaces;

namespace pst.impl.ndb
{
    class OrderedNodeDataBlockCollectionLoader : IOrderedDataBlockCollectionLoader
    {
        private readonly IBTreeKeyFinder<LBBTEntry, BID> bbtEntryFinder;

        private readonly IFactory<LBBTEntry, Maybe<IOrderedDataBlockCollection>> collectionFactory;

        public OrderedNodeDataBlockCollectionLoader(IBTreeKeyFinder<LBBTEntry, BID> bbtEntryFinder, IFactory<LBBTEntry, Maybe<IOrderedDataBlockCollection>> collectionFactory)
        {
            this.bbtEntryFinder = bbtEntryFinder;
            this.collectionFactory = collectionFactory;
        }

        public Maybe<IOrderedDataBlockCollection> Load(BID blockId)
        {
            var entry = bbtEntryFinder.Find(blockId);

            if (entry.HasNoValue)
            {
                return Maybe<IOrderedDataBlockCollection>.NoValue<IOrderedDataBlockCollection>();
            }

            return collectionFactory.Create(entry.Value);
        }
    }
}
