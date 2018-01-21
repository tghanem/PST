using pst.encodables.ndb;

namespace pst.interfaces.ndb
{
    interface INIDAllocator
    {
        NID Allocate(int type);
    }
}