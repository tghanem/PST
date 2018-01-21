using pst.interfaces.model;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextReader
    {
        TableRow[] GetAllRows(ObjectPath objectPath);
    }
}
