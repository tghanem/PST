using pst.encodables.ltp.hn;
using pst.utilities;

namespace pst.interfaces.ltp
{
    interface IHeapOnNodeItemLoader
    {
        BinaryData Load(HID id);
    }
}
