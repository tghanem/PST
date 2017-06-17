using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataBlockReader
    {
        BinaryData Read(BID blockId);
    }
}
