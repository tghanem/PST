using pst.interfaces.btree;
using pst.core;
using pst.encodables.ltp.hn;
using pst.interfaces.ltp.hn;

namespace pst.impl.ltp.bth
{
    class BTreeOnHeapNodeLoader : IBTreeNodeLoader<BTreeOnHeapNode, HID>
    {
        private readonly IHeapOnNodeItemLoader hnItemLoader;

        public BTreeOnHeapNodeLoader(IHeapOnNodeItemLoader hnItemLoader)
        {
            this.hnItemLoader = hnItemLoader;
        }

        public Maybe<BTreeOnHeapNode> LoadNode(HID nodeReference)
        {
            var hnItem = hnItemLoader.Load(nodeReference);

            if (hnItem.HasNoValue)
            {
                return Maybe<BTreeOnHeapNode>.NoValue<BTreeOnHeapNode>();
            }

            return
                Maybe<BTreeOnHeapNode>.OfValue(new BTreeOnHeapNode(hnItem.Value));
        }
    }
}
