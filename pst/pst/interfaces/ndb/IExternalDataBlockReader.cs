using pst.core;
using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ndb
{
    interface IExternalDataBlockReader
    {
        BinaryData[] Read(NID[] nodePath, Maybe<int> blockIndex);
    }
}