using pst.encodables.ndb;
using pst.encodables.ndb.btree;

namespace pst.interfaces.ndb
{
    interface IExternalDataBlockIdsReader
    {
        BID[] Read(LBBTEntry dataTreeEntry);
    }
}