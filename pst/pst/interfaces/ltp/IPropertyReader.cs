using pst.core;
using pst.encodables.ndb;

namespace pst.interfaces.ltp
{
    interface IPropertyReader
    {
        Maybe<PropertyValue> ReadProperty(BID nodeDataBlockId, BID subnodeDataBlockId, PropertyTag propertyTag);
    }

    interface ITableContextBasedPropertyReader<TRowId>
    {
        Maybe<PropertyValue> ReadProperty(BID nodeDataBlockId, BID subnodeDataBlockId, TRowId rowId, PropertyTag propertyTag);
    }
}
