using pst.encodables.ndb;

namespace pst.interfaces
{
    interface IDataBlockLoader<TBlockType>
    {
        TBlockType Load(BREF blockReference);
    }
}