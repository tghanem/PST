using pst.encodables.ndb;
using pst.utilities;

namespace pst.interfaces.blockallocation.datatree
{
    interface IDataTreeAllocator
    {
        BREF Allocate(BinaryData[] dataPerExternalBlock);
    }
}