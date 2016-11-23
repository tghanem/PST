using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ndb.btree
{
    interface IBTreePageEntryLocator<TKey, TEntry> where TEntry : class
    {
        Maybe<TEntry> FindEntry(BTPage page, TKey key);
    }
}