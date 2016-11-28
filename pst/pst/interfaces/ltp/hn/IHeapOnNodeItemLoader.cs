using pst.core;
using pst.encodables.ltp.hn;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeItemLoader
    {
        Maybe<BinaryData> Load(HID id);
    }
}
