using pst.encodables;

namespace pst.interfaces
{
    interface IDataBlockLoader<TBlockType>
    {
        TBlockType Load(BREF blockReference);
    }
}