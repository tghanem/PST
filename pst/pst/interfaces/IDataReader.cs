using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces
{
    interface IDataReader
    {
        BinaryData Read(IB byteIndex, int length);
    }
}
