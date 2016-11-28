using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeItemsExtractor
    {
        BinaryData[] Extract(BinaryData encodedHeapOnNode, int blockIndex);
    }
}
