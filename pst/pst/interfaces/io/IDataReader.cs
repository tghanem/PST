using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataReader
    {
        bool CanRead(long offset);

        BinaryData Read(long offset, int count);
    }
}
