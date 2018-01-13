using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface IRowIndexReader
    {
        TCROWID[] GetAllRowIds(NID[] nodePath);
    }
}
