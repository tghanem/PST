using pst.encodables.ltp.hn;
using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeGenerator
    {
        HID AllocateItem(BinaryData value, bool isUserRoot = false);
        BID Commit(int clientSignature);
    }
}