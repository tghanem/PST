using pst.utilities;

namespace pst.interfaces
{
    interface IPageEntriesDecoder<TEntry>
    {
        TEntry[] Decode(int pageType, int pageLevel, BinaryData encodedEntries);
    }
}