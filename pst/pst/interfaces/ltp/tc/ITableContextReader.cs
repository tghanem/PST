using pst.interfaces.messaging.model;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextReader
    {
        TableRow[] GetAllRows(NodePath nodePath);
    }
}
