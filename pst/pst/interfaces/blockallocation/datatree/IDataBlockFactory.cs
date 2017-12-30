using pst.encodables.ndb;

namespace pst.interfaces.blockallocation.datatree
{
    interface IDataBlockFactory<TDataType, TBlockType>
    {
        TBlockType Create(IB blockOffset, BID blockId, TDataType data);
    }
}