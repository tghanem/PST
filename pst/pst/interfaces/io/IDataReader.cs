using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataReader
    {
        BinaryData Read(IB byteIndex, int count);
    }
}
