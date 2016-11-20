using pst.encodables;

namespace pst.interfaces
{
    interface IBTreePageEntryLocator<TKey, TEntry>
    {
        TEntry FindEntry(BTPage page, TKey key);
    }
}
