using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface IDataBlockEntryFinder
    {
        Maybe<DataBlockEntry> Find(BID blockId);
    }
}