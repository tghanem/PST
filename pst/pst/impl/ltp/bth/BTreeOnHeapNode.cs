using pst.utilities;

namespace pst.impl.ltp.bth
{
    class BTreeOnHeapNode
    {
        public BinaryData Records { get; }

        public BTreeOnHeapNode(BinaryData records)
        {
            Records = records;
        }
    }
}
