using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ndb
{
    interface IDataBlockReader
    {
        BinaryData Read(BID blockId);
    }
}
