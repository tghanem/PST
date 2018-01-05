using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ndb
{
    interface IExternalDataBlockReader
    {
        BinaryData Read(NID[] nodePath, int blockIndex);
    }
}