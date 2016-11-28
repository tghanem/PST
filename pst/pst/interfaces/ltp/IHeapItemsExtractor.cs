using pst.utilities;

namespace pst.interfaces.ltp
{
    interface IHeapItemsExtractor
    {
        BinaryData[] Extract(BinaryData encodedHeapOnNode, int blockIndex);
    }
}
