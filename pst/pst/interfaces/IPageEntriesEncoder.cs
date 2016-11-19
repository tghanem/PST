using pst.utilities;

namespace pst.interfaces
{
    interface IPageEntriesEncoder<TEntry>
    {
        BinaryData Encode(int pageType, int pageLevel, TEntry[] entries);
    }
}
