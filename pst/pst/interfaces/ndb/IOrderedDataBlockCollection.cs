using pst.encodables.ndb.blocks.data;

namespace pst.interfaces.ndb
{
    interface IOrderedDataBlockCollection
    {
        ExternalDataBlock GetDataBlockAt(int blockIndex);
    }
}
