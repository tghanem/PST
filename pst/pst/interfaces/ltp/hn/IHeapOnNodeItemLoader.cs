using pst.encodables.ltp.hn;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeItemLoader
    {
        BinaryData Load(HID id);
    }
}
