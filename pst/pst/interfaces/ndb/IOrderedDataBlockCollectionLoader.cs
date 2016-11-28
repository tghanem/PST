using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface IOrderedDataBlockCollectionLoader
    {
        Maybe<IOrderedDataBlockCollection> Load(BID blockId);
    }
}
