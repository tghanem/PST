using pst.core;
using pst.encodables.ltp.tc;
using pst.encodables.ndb;

namespace pst.interfaces.ltp.tc
{
    interface ITableContextReader<TRowId>
    {
        TCROWID[] GetAllRowIds(BID nodeBlockId);

        TableRow[] GetAllRows(BID nodeBlockId, BID subnodeBlockId);

        Maybe<PropertyValue> ReadProperty(BID nodeBlockId, BID subnodeBlockId, TRowId rowId, PropertyTag propertyTag);
    }
}
