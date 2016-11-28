using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface IDataBlockLoader<TBlockType> where TBlockType : class
    {
        Maybe<TBlockType> Load(BID blockId);
    }
}
