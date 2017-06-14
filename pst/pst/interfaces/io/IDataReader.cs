using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataReader
    {
        BinaryData Read(long offset, int count);
    }
}
