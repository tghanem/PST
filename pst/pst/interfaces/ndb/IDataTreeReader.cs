using pst.core;
using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.ndb
{
    interface IDataTreeReader
    {
        BinaryData[] Read(NID[] nodePath, Maybe<int> blockIndex);
    }
}