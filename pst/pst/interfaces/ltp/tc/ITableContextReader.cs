using pst.interfaces.ndb;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextReader
    {
        TableRow[] GetAllRows(NodePath nodePath);
    }
}
