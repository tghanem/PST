using pst.core;
using pst.encodables;

namespace pst.interfaces
{
    interface IBTreePageEntryLocator<TKey, TEntry> where TEntry : class
    {
        Maybe<TEntry> FindEntry(BTPage page, TKey key);
    }
}