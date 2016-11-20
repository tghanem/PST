using pst.encodables;
using pst.utilities;

namespace pst.interfaces
{
    interface IDataReader
    {
        BinaryData Read(IB byteIndex, int length);
    }
}
