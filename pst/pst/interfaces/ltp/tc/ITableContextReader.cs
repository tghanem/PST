using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextReader
    {
        TableRow[] GetAllRows(BID nodeBlockId, BID subnodeBlockId);
    }
}
