using pst.utilities;

namespace pst.interfaces.ndb
{
    interface IExternalDataBlockReader
    {
        BinaryData Read(NodePath nodePath, int blockIndex);
    }
}