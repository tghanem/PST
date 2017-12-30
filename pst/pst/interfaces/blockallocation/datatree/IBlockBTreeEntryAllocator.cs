using pst.encodables.ndb;

namespace pst.interfaces.blockallocation.datatree
{
    interface IBlockBTreeEntryAllocator
    {
        BID Allocate(IB blockOffset, int rawDataSize, bool internalBlock);
    }
}