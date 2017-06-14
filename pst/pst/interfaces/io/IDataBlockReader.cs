using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.io
{
    interface IDataBlockReader
    {
        BinaryData Read(BID blockId);
    }

    interface IDataBlockReader<TBlockId>
    {
        BinaryData Read(TBlockId blockId, int blockSize);
    }
}
