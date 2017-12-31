using pst.impl.ltp.hn;
using pst.utilities;

namespace pst.interfaces.ltp.hn
{
    interface IHeapOnNodeEncoder
    {
        BinaryData[] Encode(ExternalDataBlockForHeapOnNode[] blocks, int clientSignature);
    }
}